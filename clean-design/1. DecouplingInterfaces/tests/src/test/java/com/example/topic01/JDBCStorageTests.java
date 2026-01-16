package com.example.topic01;

import org.junit.jupiter.api.*;

import java.sql.*;
import java.util.Arrays;
import java.util.List;

import static org.assertj.core.api.BDDAssertions.then;

@TestInstance(TestInstance.Lifecycle.PER_CLASS)
public class JDBCStorageTests {
    private Connection connection;
    private JDBCStorage storage;

    @BeforeAll
    void initDb() throws Exception {
        connection = DriverManager.getConnection("jdbc:h2:mem:test;DB_CLOSE_DELAY=-1");
        try (Statement stmt = connection.createStatement()) {
            stmt.execute("CREATE TABLE storage (id INT AUTO_INCREMENT PRIMARY KEY, data VARCHAR(255))");
        }
        storage = new JDBCStorage(connection);
    }

    @BeforeEach
    void cleanDb() throws SQLException {
        try (Statement stmt = connection.createStatement()) {
            stmt.execute("DELETE FROM storage");
            stmt.execute("TRUNCATE TABLE storage RESTART IDENTITY");
        }
    }

    @AfterAll
    void tearDown() throws SQLException {
        connection.close();
    }

    @Test
    public void GivenEmptyDatabase_WhenInsertingNewEntry_ThenItMustBeAvailableUnderIdZero() {
        // Given

        // When
        String entry = "hello";
        storage.save(entry);

        // Then
        then(storage.retrieve(0)).isEqualTo(entry);
    }

    @Test
    public void GivenEmptyDatabase_WhenInsertingMultipleEntries_ThenTheyMustBeRetrievableUnderCorrespondingIndexes() {
        // Given

        // When
        List<String> entries = Arrays.asList("foo", "bar", "baz");
        entries.forEach(storage::save);

        // Then
        for (int i = 0; i < entries.size(); i++) {
            then(storage.retrieve(i)).isEqualTo(entries.get(i));
        }
    }

    @Test
    public void GivenEmptyDatabase_WhenRetrievingWithoutSaving_ThenItMustReturnNull() {
        // Given

        // When
        String result = storage.retrieve(0);

        // Then
        then(result).isNull();
    }

    @Test
    public void GivenDatabaseWithEntries_WhenRetrievingOutOfBoundsIndex_ThenItMustReturnNull() {
        // Given
        storage.save("one");
        storage.save("two");

        // When
        String result = storage.retrieve(5);

        // Then
        then(result).isNull();
    }

    @Test
    public void GivenDatabase_WhenSavingEmptyString_ThenItMustBeRetrievable() {
        // Given

        // When
        storage.save("");
        String result = storage.retrieve(0);

        // Then
        then(result).isEqualTo("");
    }

    @Test
    public void GivenDatabaseWithPreloadedEntries_WhenRetrievingThem_ThenTheyMustBeReturnableByCorrectIndex() throws SQLException {
        // Given
        try (PreparedStatement stmt = connection.prepareStatement("INSERT INTO storage (data) VALUES (?), (?), (?)")) {
            stmt.setString(1, "pre1");
            stmt.setString(2, "pre2");
            stmt.setString(3, "pre3");
            stmt.executeUpdate();
        }

        // When
        String first = storage.retrieve(0);
        String second = storage.retrieve(1);
        String third = storage.retrieve(2);

        // Then
        then(first).isEqualTo("pre1");
        then(second).isEqualTo("pre2");
        then(third).isEqualTo("pre3");
    }
}
