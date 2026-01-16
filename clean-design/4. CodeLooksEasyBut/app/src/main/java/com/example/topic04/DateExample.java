package com.example.topic04;

import java.time.*;
import java.time.format.DateTimeFormatter;

public class DateExample {

    // Original approach drawbacks:
    // 1. The custom date format is ambiguous
    // 2. Prone to errors due to timezone misinterpretation or incorrect format parsing.

    // Improved approach:
    // Require ISO 8601 format for UTC based time (e.g., "2024-05-13T14:30:00Z") to avoid ambiguities.
    private static final DateTimeFormatter ISO_FORMATTER = DateTimeFormatter.ISO_INSTANT;

    public static ZonedDateTime parseIsoUtcAndConvert(String isoUtcDateString, ZoneId targetZoneId) {
        Instant instant = Instant.parse(isoUtcDateString);
        return instant.atZone(targetZoneId);
    }

    public static void main(String[] args) {
        String isoUtcDateString = "2024-05-13T14:30:00Z";

        // convert UTC date to local timezone (e.g., Europe/Berlin)
        ZonedDateTime berlinTime = parseIsoUtcAndConvert(isoUtcDateString, ZoneId.of("Europe/Berlin"));
    }
}
