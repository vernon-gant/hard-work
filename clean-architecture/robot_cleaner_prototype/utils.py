from result import Result, Ok, Err

from value_objects import NonEmptyString

def parse_int(raw: NonEmptyString) -> Result[int, str]:
    clean = raw.value.strip()

    if clean.lstrip("-").isdigit() and clean.count("-") <= 1:
        return Ok(int(clean))
    
    return Err(f"'{raw.value}' is not a valid integer.")