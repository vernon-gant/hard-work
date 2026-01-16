package com.example.topic01;

public interface Storage {
    void save(String data);
    String retrieve(int id);
}