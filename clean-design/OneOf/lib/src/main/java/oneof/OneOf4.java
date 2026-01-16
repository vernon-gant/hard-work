package oneof;

import java.util.Objects;
import java.util.function.Consumer;
import java.util.function.Function;

public final class OneOf4<T0, T1, T2, T3> implements OneOf {

    private final Object value;
    private final int index;          // 0..3

    private OneOf4(int idx, Object v) {
        this.index = idx;
        this.value = v;
    }

    public static <T0, T1, T2, T3> OneOf4<T0, T1, T2, T3> ofT0(T0 v) {
        return new OneOf4<>(0, v);
    }

    public static <T0, T1, T2, T3> OneOf4<T0, T1, T2, T3> ofT1(T1 v) {
        return new OneOf4<>(1, v);
    }

    public static <T0, T1, T2, T3> OneOf4<T0, T1, T2, T3> ofT2(T2 v) {
        return new OneOf4<>(2, v);
    }

    public static <T0, T1, T2, T3> OneOf4<T0, T1, T2, T3> ofT3(T3 v) {
        return new OneOf4<>(3, v);
    }

    @Override
    public Object value() {
        return value;
    }

    @Override
    public int index() {
        return index;
    }

    public boolean isT0() {
        return index == 0;
    }

    public boolean isT1() {
        return index == 1;
    }

    public boolean isT2() {
        return index == 2;
    }

    public boolean isT3() {
        return index == 3;
    }

    @SuppressWarnings("unchecked")
    public T0 asT0() {
        ensure(0);
        return (T0) value;
    }

    @SuppressWarnings("unchecked")
    public T1 asT1() {
        ensure(1);
        return (T1) value;
    }

    @SuppressWarnings("unchecked")
    public T2 asT2() {
        ensure(2);
        return (T2) value;
    }

    @SuppressWarnings("unchecked")
    public T3 asT3() {
        ensure(3);
        return (T3) value;
    }

    public void switchTo(Consumer<? super T0> c0,
                         Consumer<? super T1> c1,
                         Consumer<? super T2> c2,
                         Consumer<? super T3> c3) {
        switch (index) {
            case 0 -> c0.accept(asT0());
            case 1 -> c1.accept(asT1());
            case 2 -> c2.accept(asT2());
            case 3 -> c3.accept(asT3());
            default -> throw unexpected();
        }
    }

    public <R> R match(Function<? super T0, ? extends R> f0,
                       Function<? super T1, ? extends R> f1,
                       Function<? super T2, ? extends R> f2,
                       Function<? super T3, ? extends R> f3) {
        return switch (index) {
            case 0 -> f0.apply(asT0());
            case 1 -> f1.apply(asT1());
            case 2 -> f2.apply(asT2());
            case 3 -> f3.apply(asT3());
            default -> throw unexpected();
        };
    }

    public <R> OneOf4<R, T1, T2, T3> mapT0(Function<? super T0, ? extends R> f) {
        Objects.requireNonNull(f);
        return switch (index) {
            case 0 -> ofT0(f.apply(asT0()));
            case 1 -> ofT1(asT1());
            case 2 -> ofT2(asT2());
            case 3 -> ofT3(asT3());
            default -> throw unexpected();
        };
    }

    public <R> OneOf4<T0, R, T2, T3> mapT1(Function<? super T1, ? extends R> f) {
        Objects.requireNonNull(f);
        return switch (index) {
            case 0 -> ofT0(asT0());
            case 1 -> ofT1(f.apply(asT1()));
            case 2 -> ofT2(asT2());
            case 3 -> ofT3(asT3());
            default -> throw unexpected();
        };
    }

    public <R> OneOf4<T0, T1, R, T3> mapT2(Function<? super T2, ? extends R> f) {
        Objects.requireNonNull(f);
        return switch (index) {
            case 0 -> ofT0(asT0());
            case 1 -> ofT1(asT1());
            case 2 -> ofT2(f.apply(asT2()));
            case 3 -> ofT3(asT3());
            default -> throw unexpected();
        };
    }

    public <R> OneOf4<T0, T1, T2, R> mapT3(Function<? super T3, ? extends R> f) {
        Objects.requireNonNull(f);
        return switch (index) {
            case 0 -> ofT0(asT0());
            case 1 -> ofT1(asT1());
            case 2 -> ofT2(asT2());
            case 3 -> ofT3(f.apply(asT3()));
            default -> throw unexpected();
        };
    }

    private void ensure(int exp) {
        if (index != exp) throw new IllegalStateException("Value is T" + index);
    }

    private IllegalStateException unexpected() {
        return new IllegalStateException("Bad index " + index);
    }

    @Override
    public boolean equals(Object o) {
        return this == o || (o instanceof OneOf4<?, ?, ?, ?> other && index == other.index && Objects.equals(value, other.value));
    }

    @Override
    public int hashCode() {
        return Objects.hash(index, value);
    }

    @Override
    public String toString() {
        return "T" + index + "(" + value + ")";
    }
}
