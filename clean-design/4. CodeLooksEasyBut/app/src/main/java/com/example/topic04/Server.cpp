#include "TwMailerServer.h"
#include "ClientSession.h"
#include <iostream>
#include <netinet/in.h>
#include <sys/socket.h>
#include <unistd.h>
#include <thread>
#include <stdexcept>

// Initially was a project in a university where we implemented socket communication. At that time I just hardcoded
// all strings in constructor and had no DI. Now looks way better

struct ServerConfig {
    uint16_t port;
    int maxConnections;
    bool suppressErrors;
};

TwMailerServer::TwMailerServer(
    const ServerConfig& config,
    std::shared_ptr<IAuthenticationService> authService,
    std::shared_ptr<IMailService> mailService,
    std::shared_ptr<ICommandHandler> commandHandler
) :
    running_(false),
    serverSocket_(-1),
    config_(config),
    authService_(std::move(authService)),
    mailService_(std::move(mailService)),
    commandHandler_(std::move(commandHandler))
{

}

TwMailerServer::~TwMailerServer() {
    stop();
}

void TwMailerServer::start() {
    if (running_) return;

    serverSocket_ = createServerSocket(config_.port);
    listenOrThrow(serverSocket_, config_.maxConnections);

    running_ = true;
    if (!config_.suppressErrors)
        std::cout << "Server started on port " << config_.port << std::endl;

    std::thread(&TwMailerServer::acceptConnections, this).detach();
}

void TwMailerServer::stop() {
    if (!running_) return;
    running_ = false;
    if (serverSocket_ != -1) {
        ::close(serverSocket_);
        serverSocket_ = -1;
    }
    cleanupConnections();
}

bool TwMailerServer::isRunning() const {
    return running_;
}

int TwMailerServer::createServerSocket(uint16_t port) {
    int sock = ::socket(AF_INET, SOCK_STREAM, 0);
    if (sock < 0) throw std::runtime_error("socket() failed");

    int reuse = 1;
    if (::setsockopt(sock, SOL_SOCKET, SO_REUSEADDR, &reuse, sizeof(reuse)) < 0)
        throw std::runtime_error("setsockopt(SO_REUSEADDR) failed");

    sockaddr_in addr{};
    addr.sin_family = AF_INET;
    addr.sin_addr.s_addr = INADDR_ANY;
    addr.sin_port = htons(port);
    if (::bind(sock, reinterpret_cast<sockaddr*>(&addr), sizeof(addr)) < 0)
        throw std::runtime_error("bind() failed");

    return sock;
}

void TwMailerServer::listenOrThrow(int sock, int backlog) {
    if (::listen(sock, backlog) < 0)
        throw std::runtime_error("listen() failed");
}

void TwMailerServer::acceptConnections() {
    while (running_) {
        int clientSock = ::accept(serverSocket_, nullptr, nullptr);
        if (clientSock < 0) continue;
        std::thread(&TwMailerServer::handleClient, this, clientSock).detach();
    }
}

void TwMailerServer::handleClient(int clientSocket) {
    auto session = std::make_shared<ClientSession>(clientSocket, commandHandler_);
    session->start();
}

void TwMailerServer::cleanupConnections() {
    std::lock_guard<std::mutex> lock(clientThreadsMutex_);
    for (auto& t : clientThreads_)
        if (t.joinable()) t.join();
    clientThreads_.clear();
}
