package com.example.topic02;

import oneof.OneOf4;
import oneof.types.Empty;
import oneof.types.NegativeValuesPresent;
import oneof.types.NullValuesPresent;

import java.util.List;

public class GradeCalculator {
    public static OneOf4<Double, Empty, NegativeValuesPresent, NullValuesPresent> calculateAverage(List<Integer> grades) {
        if (grades == null || grades.isEmpty())
            return OneOf4.ofT1(new Empty());

        long sum = 0;
        int cnt = 0;

        for (Integer g : grades) {
            if (g == null)
                return OneOf4.ofT3(new NullValuesPresent());
            if (g < 0)
                return OneOf4.ofT2(new NegativeValuesPresent());

            sum += g;          // long — защита от переполнения
            cnt++;
        }

        double avg = sum / (double) cnt;
        return OneOf4.ofT0(avg);
    }
}