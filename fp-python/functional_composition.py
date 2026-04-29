from pymonad.tools import curry


@curry(2)
def tag(tag_symbol: str, text: str) -> str:
    return f"<{tag_symbol}>{text}</{tag_symbol}>"


bold = tag("b")
italic = tag("i")


@curry(3)
def tag_extended(tag_symbol: str, attrs: dict, text: str) -> str:
    attrs_str = " ".join(f'{k}="{v}"' for k, v in attrs.items())
    return f"<{tag_symbol} {attrs_str}>{text}</{tag_symbol}>"
