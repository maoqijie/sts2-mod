#!/usr/bin/env python3
"""列出 Godot 4 PCK 目录表中的文件路径。"""

from __future__ import annotations

import struct
import sys
from pathlib import Path


HEADER_SIZE = 0x70
DIRECTORY_OFFSET_POSITION = 0x20
ENTRY_METADATA_SIZE = 36


def read_u32(data: bytes, offset: int) -> int:
    return struct.unpack_from("<I", data, offset)[0]


def read_u64(data: bytes, offset: int) -> int:
    return struct.unpack_from("<Q", data, offset)[0]


def list_pck_files(path: Path) -> list[str]:
    data = path.read_bytes()
    if len(data) < HEADER_SIZE or data[:4] != b"GDPC":
        raise ValueError(f"不是受支持的 Godot PCK 文件：{path}")

    directory_offset = read_u64(data, DIRECTORY_OFFSET_POSITION)
    if directory_offset <= 0 or directory_offset >= len(data):
        raise ValueError(f"PCK 目录偏移异常：{directory_offset}")

    offset = directory_offset
    entry_count = read_u32(data, offset)
    offset += 4

    paths: list[str] = []
    for _ in range(entry_count):
        path_length = read_u32(data, offset)
        offset += 4
        raw_path = data[offset : offset + path_length]
        offset += path_length
        offset += ENTRY_METADATA_SIZE
        paths.append(raw_path.rstrip(b"\0").decode("utf-8"))

    if offset != len(data):
        raise ValueError(f"PCK 目录读取未对齐：offset={offset}, size={len(data)}")

    return paths


def main() -> int:
    if len(sys.argv) != 2:
        print("用法：list_pck_files.py <file.pck>", file=sys.stderr)
        return 2

    for file_path in list_pck_files(Path(sys.argv[1])):
        print(file_path)

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
