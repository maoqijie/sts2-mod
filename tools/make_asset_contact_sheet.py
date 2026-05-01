from __future__ import annotations

from pathlib import Path
import importlib.util

from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[1]
SRC = ROOT / "docs/assets/pengfu-nailong/generated/v4-locked"
OUT = ROOT / "docs/assets/pengfu-nailong/generated/v4-locked/contact-sheet.png"
PROCESSOR = ROOT / "tools/process_imagegen_assets.py"


def selected_sources() -> list[Path]:
    spec = importlib.util.spec_from_file_location("process_imagegen_assets", PROCESSOR)
    if spec is None or spec.loader is None:
        return sorted(SRC.rglob("*.master.png"))
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    ids = (
        module.CARD_IDS
        | module.POWER_IDS
        | module.RELIC_IDS
        | set(module.DIRECT_UI_SIZES)
        | set(module.ROOT_IMAGE_SIZES)
        | {"character_model_master"}
    )
    sources = module.all_sources()
    return [sources[asset_id] for asset_id in sorted(ids) if asset_id in sources]


def open_thumb(path: Path, size: tuple[int, int]) -> Image.Image:
    img = Image.open(path).convert("RGBA")
    img.thumbnail(size, Image.Resampling.LANCZOS)
    out = Image.new("RGBA", size, (30, 28, 24, 255))
    out.alpha_composite(img, ((size[0] - img.width) // 2, (size[1] - img.height) // 2))
    return out.convert("RGB")


def main() -> None:
    paths = selected_sources()
    tile_w, tile_h = 240, 210
    label_h = 38
    cols = 5
    rows = max(1, (len(paths) + cols - 1) // cols)
    sheet = Image.new("RGB", (cols * tile_w, rows * (tile_h + label_h)), (18, 16, 14))
    draw = ImageDraw.Draw(sheet)
    font = ImageFont.load_default()
    for index, path in enumerate(paths):
        x = index % cols * tile_w
        y = index // cols * (tile_h + label_h)
        sheet.paste(open_thumb(path, (tile_w, tile_h)), (x, y))
        label = path.name.removesuffix(".master.png")
        draw.text((x + 8, y + tile_h + 6), label[:34], fill=(238, 226, 190), font=font)
    OUT.parent.mkdir(parents=True, exist_ok=True)
    sheet.save(OUT)
    print(OUT)


if __name__ == "__main__":
    main()
