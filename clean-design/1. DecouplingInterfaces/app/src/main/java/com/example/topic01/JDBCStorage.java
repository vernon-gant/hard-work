package com.example.topic01;

import java.sql.*;

public class JDBCStorage implements Storage {

    private final Connection connection;

    public JDBCStorage(Connection connection) {
        this.connection = connection;
    }

    @Override
    public void save(String data) {
        String sql = "INSERT INTO storage (data) VALUES (?)";
        try (PreparedStatement stmt = connection.prepareStatement(sql)) {
            stmt.setString(1, data);
            stmt.executeUpdate();
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public String retrieve(int id) {
        String sql = "SELECT data FROM storage WHERE id = ?";
        try (PreparedStatement query = connection.prepareStatement(sql)) {
            query.setInt(1, id + 1);
            try (ResultSet rs = query.executeQuery()) {
                return rs.next() ? rs.getString("data") : null;
            }
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }
}