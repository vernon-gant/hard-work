package com.example.topic02;

public class BankAccount {
    private double balance;

    public BankAccount(double initialBalance) {
        balance = initialBalance;
    }

    public void deposit(double amount) {
        if (amount < 0) return;

        balance += amount;
    }

    public void withdraw(double amount) {
        if (amount < 0 || amount > balance) return;

        balance -= amount;
    }

    public double getBalance() {
        return balance;
    }
}