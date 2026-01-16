package oneof;

import java.util.Objects;
import java.util.function.Consumer;
import java.util.function.Function;

public final class OneOf2<T0, T1> implements OneOf {

    private final Object value;
    private final int index;

    private OneOf2(int idx, Object v) {
        this.index = idx;
        this.value = v;
    }

    public static <T0, T1> OneOf2<T0, T1> ofT0(T0 v) {
        return new OneOf2<>(0, v);
    }

    public static <T0, T1> OneOf2<T0, T1> ofT1(T1 v) {
        return new OneOf2<>(1, v);
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

    public void switchTo(Consumer<? super T0> c0, Consumer<? super T1> c1) {
        switch (index) {
            case 0 -> c0.accept(asT0());
            case 1 -> c1.accept(asT1());
            default -> throw unexpected();
        }
    }

    public <R> R match(Function<? super T0, ? extends R> f0,
                       Function<? super T1, ? extends R> f1) {
        return switch (index) {
            case 0 -> f0.apply(asT0());
            case 1 -> f1.apply(asT1());
            default -> throw unexpected();
        };
    }

    private void ensure(int expected) {
        if (index != expected) throw new IllegalStateException("Value is T" + index);
    }

    private IllegalStateException unexpected() {
        return new IllegalStateException("Bad index " + index);
    }

    @Override
    public boolean equals(Object o) {
        return this == o ||
                (o instanceof OneOf2<?, ?> other &&
                        index == other.index &&
                        Objects.equals(value, other.value));
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