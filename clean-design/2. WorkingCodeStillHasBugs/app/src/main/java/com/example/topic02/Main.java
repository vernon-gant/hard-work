package com.example.topic02;

public class Main {
    public static void main(String[] args) {
        BankAccount account = new BankAccount(1_000.0);

        System.out.println("Initial balance: " + account.getBalance());

        account.deposit(500.0);
        System.out.println("After depositing 500: " + account.getBalance());

        account.withdraw(200.0);
        System.out.println("After withdrawing 200: " + account.getBalance());

        account.deposit(-50.0);
        System.out.println("After depositing -50: " + account.getBalance());

        account.withdraw(2_000.0);
        System.out.println("After withdrawing 2000: " + account.getBalance());
    }
}