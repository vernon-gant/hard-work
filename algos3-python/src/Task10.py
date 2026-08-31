LETTERS = "abcdefgh"
DIGITS = "0123456789"
CODES_PER_LETTER = 100


class ksort:
    def __init__(self):
        self.items = [None] * (len(LETTERS) * CODES_PER_LETTER)

    def index(self, s):
        if not isinstance(s, str) or len(s) != 3:
            return -1

        letter = s[0]
        digits = s[1:]

        if letter not in LETTERS:
            return -1
        if any(character not in DIGITS for character in digits):
            return -1

        letter_offset = LETTERS.index(letter)
        return letter_offset * CODES_PER_LETTER + int(digits)

    def add(self, s):
        position = self.index(s)
        if position == -1:
            return False

        self.items[position] = s
        return True
