from __future__ import annotations

from pathlib import Path

from collections import deque

from PIL import Image, ImageDraw, ImageFilter, ImageOps

ROOT = Path(__file__).resolve().parents[1]
IMAGES = ROOT / "PengfuNailongMod/images"
SOURCE_ROOTS = (
    (0, ROOT / "docs/assets/pengfu-nailong/generated/v3-imagegen"),
    (1, ROOT / "docs/assets/pengfu-nailong/generated/v4-locked"),
    (2, ROOT / "docs/assets/pengfu-nailong/generated/v5-combat-fix"),
)

SOURCE_HINTS = {
    "hand_point_nailong": "/ui/",
    "hand_rock_nailong": "/ui/",
    "hand_paper_nailong": "/ui/",
    "hand_scissors_nailong": "/ui/",
}

CARD_IDS = {
    "angry_belly_slap",
    "angry_puffed_cheeks",
    "belly_bounce",
    "belly_charge",
    "belly_charge_energy",
    "belly_cushion",
    "belly_hug_curl",
    "belly_pad",
    "changing_expression_pack",
    "cry_to_laugh",
    "curl_up",
    "deep_breath",
    "emotion_explosion",
    "emotion_kaleidoscope",
    "emotional_breakdown",
    "emotional_outburst",
    "emotional_overload",
    "everyone_laughs",
    "expression_masks",
    "expression_roulette",
    "face_chaos",
    "face_master",
    "five_emotions",
    "frightened_bounce_away",
    "hard_to_hold_laugh",
    "huge_belly_rebound",
    "hundred_faces",
    "laugh_or_fright_attack",
    "laughing_counter",
    "laughing_endure",
    "nailong_defend",
    "nailong_strike",
    "out_of_control_performance",
    "overconfident_draw",
    "overreaction",
    "pengfu_combo",
    "rolling_defense_line",
    "scared_smack",
    "smug_hands_on_hips",
    "smug_hit_strength",
    "sneeze_face_change",
    "soft_rebound",
    "startled_flailing_slap",
    "teary_block",
    "teary_weak_aura",
    "uncontrolled_face",
}

POWER_IDS = {
    "expression_aggrieved",
    "expression_angry",
    "expression_fright",
    "expression_laugh",
    "expression_smug",
}

RELIC_IDS = {
    "belly_amulet",
    "changing_nailong_belly",
    "chaos_button",
    "nailong_belly",
    "reverse_sticker",
}

CHARUI_SIZES = {
    "character_model": (420, 560),
    "character_icon": (128, 128),
    "char_select": (132, 195),
    "char_select_locked": (132, 195),
    "map_marker": (128, 128),
    "dialog": (360, 430),
    "merchant": (430, 470),
    "rest_site": (460, 420),
}

DIRECT_UI_SIZES = {
    "char_select_bg_nailong": (1920, 1080),
    "energy_counter_nailong": (128, 128),
    "big_energy": (74, 74),
    "text_energy": (24, 24),
    "trail_nailong": (96, 96),
    "hand_point_nailong": (180, 180),
    "hand_rock_nailong": (180, 180),
    "hand_paper_nailong": (180, 180),
    "hand_scissors_nailong": (180, 180),
}

RIG_PARTS = {
    "body": [
        ("ellipse", (62, 118, 348, 485), 255),
        ("polygon", [(84, 135), (170, 92), (316, 138), (344, 265), (75, 270)], 255),
        ("rectangle", (0, 0, 420, 86), 0),
        ("ellipse", (116, 288, 206, 377), 0),
        ("ellipse", (248, 280, 340, 370), 0),
    ],
    "head": [("ellipse", (140, 40, 320, 198), 255)],
    "belly": [("ellipse", (124, 198, 315, 407), 255)],
    "left_arm": [("polygon", [(92, 165), (159, 157), (191, 318), (135, 342), (82, 266)], 255)],
    "right_arm": [("polygon", [(270, 162), (342, 213), (333, 337), (272, 325), (252, 220)], 255)],
    "left_hand": [("ellipse", (118, 296, 204, 373), 255)],
    "right_hand": [("ellipse", (250, 286, 338, 366), 255)],
    "left_leg": [("polygon", [(120, 360), (205, 360), (205, 520), (82, 520), (110, 445)], 255)],
    "right_leg": [("polygon", [(225, 360), (327, 360), (354, 520), (230, 520), (215, 430)], 255)],
}

ROOT_IMAGE_SIZES = {
    "mod_image": (420, 420),
}


def all_sources() -> dict[str, Path]:
    sources: dict[str, Path] = {}
    weights: dict[str, tuple[int, int, float]] = {}
    for priority, root in SOURCE_ROOTS:
        if not root.exists():
            continue
        for path in root.rglob("*.master.png"):
            asset_id = path.name.removesuffix(".master.png")
            hint = SOURCE_HINTS.get(asset_id)
            hint_score = int(hint is not None and hint in path.as_posix())
            weight = (priority, hint_score, path.stat().st_mtime)
            if weight >= weights.get(asset_id, (-1, -1, 0.0)):
                sources[asset_id] = path
                weights[asset_id] = weight
    return sources


def open_rgba(path: Path) -> Image.Image:
    return Image.open(path).convert("RGBA")


def crop_resize(img: Image.Image, size: tuple[int, int]) -> Image.Image:
    return ImageOps.fit(img, size, method=Image.Resampling.LANCZOS, centering=(0.5, 0.5))


def clear_edge_light_background(img: Image.Image) -> Image.Image:
    img = img.convert("RGBA")
    pixels = img.load()
    width, height = img.size
    visited: set[tuple[int, int]] = set()
    queue: deque[tuple[int, int]] = deque()

    for x in range(width):
        queue.append((x, 0))
        queue.append((x, height - 1))
    for y in range(1, height - 1):
        queue.append((0, y))
        queue.append((width - 1, y))

    while queue:
        x, y = queue.popleft()
        if (x, y) in visited:
            continue
        visited.add((x, y))

        r, g, b, a = pixels[x, y]
        if a == 0:
            passable = true_transparent = True
        else:
            max_c = max(r, g, b)
            min_c = min(r, g, b)
            passable = max_c >= 225 and max_c - min_c <= 34
            true_transparent = False

        if not passable:
            continue
        if not true_transparent:
            pixels[x, y] = (r, g, b, 0)

        if x > 0:
            queue.append((x - 1, y))
        if x < width - 1:
            queue.append((x + 1, y))
        if y > 0:
            queue.append((x, y - 1))
        if y < height - 1:
            queue.append((x, y + 1))

    return img


def fit_transparent(img: Image.Image, size: tuple[int, int], pad: float = 0.04) -> Image.Image:
    img = trim_alpha(img)
    max_w = int(size[0] * (1 - pad * 2))
    max_h = int(size[1] * (1 - pad * 2))
    img.thumbnail((max_w, max_h), Image.Resampling.LANCZOS)
    out = Image.new("RGBA", size, (0, 0, 0, 0))
    out.alpha_composite(img, ((size[0] - img.width) // 2, (size[1] - img.height) // 2))
    return out


def trim_alpha(img: Image.Image) -> Image.Image:
    alpha = img.getchannel("A")
    box = alpha.getbbox()
    return img.crop(box) if box else img


def save_png(img: Image.Image, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    img.save(path)


def save_card(asset_id: str, src: Path) -> None:
    img = open_rgba(src)
    save_png(crop_resize(img, (1000, 760)), IMAGES / f"card_portraits/big/{asset_id}.png")
    save_png(crop_resize(img, (250, 190)), IMAGES / f"card_portraits/{asset_id}.png")


def save_power(asset_id: str, src: Path) -> None:
    img = clear_edge_light_background(open_rgba(src))
    save_png(fit_transparent(img, (330, 330), pad=0.08), IMAGES / f"powers/{asset_id}.png")


def save_relic(asset_id: str, src: Path) -> None:
    img = fit_transparent(clear_edge_light_background(open_rgba(src)), (330, 330), pad=0.1)
    save_png(img, IMAGES / f"relics/big/{asset_id}.png")
    small = img.resize((94, 94), Image.Resampling.LANCZOS)
    save_png(small, IMAGES / f"relics/{asset_id}.png")
    save_png(make_outline(small), IMAGES / f"relics/{asset_id}_outline.png")


def save_direct_ui(asset_id: str, src: Path) -> None:
    size = DIRECT_UI_SIZES[asset_id]
    save_png(crop_resize(open_rgba(src), size), IMAGES / f"charui/{asset_id}.png")


def save_rig_parts(model: Image.Image) -> None:
    for part_id, shapes in RIG_PARTS.items():
        mask = Image.new("L", model.size, 0)
        draw = ImageDraw.Draw(mask)
        for kind, data, fill in shapes:
            getattr(draw, kind)(data, fill=fill)
        alpha = Image.composite(model.getchannel("A"), Image.new("L", model.size, 0), mask)
        part = model.copy()
        part.putalpha(alpha)
        box = part.getbbox()
        if box:
            save_png(part.crop(box), IMAGES / f"charui/rig/{part_id}.png")
    save_png(make_rig_underlay(model), IMAGES / "charui/rig/base_underlay.png")


def make_rig_underlay(model: Image.Image) -> Image.Image:
    mask = Image.new("L", model.size, 0)
    draw = ImageDraw.Draw(mask)
    for part_id in ("left_hand", "right_hand"):
        for kind, data, fill in RIG_PARTS[part_id]:
            if fill:
                getattr(draw, kind)(data, fill=255)
    mask = mask.filter(ImageFilter.MaxFilter(5)).filter(ImageFilter.GaussianBlur(3))
    softened = model.filter(ImageFilter.GaussianBlur(10))
    underlay = Image.composite(softened, model, mask)
    underlay.putalpha(model.getchannel("A"))
    return underlay


def save_root_image(asset_id: str, src: Path) -> None:
    size = ROOT_IMAGE_SIZES[asset_id]
    save_png(crop_resize(open_rgba(src), size), ROOT / f"PengfuNailongMod/{asset_id}.png")


def make_outline(img: Image.Image) -> Image.Image:
    alpha = img.getchannel("A")
    if not alpha.getbbox():
        gray = ImageOps.grayscale(img).point(lambda p: 255 if p < 245 else 0)
        alpha = gray
    outline = alpha.filter(ImageFilter.MaxFilter(7))
    out = Image.new("RGBA", img.size, (18, 16, 12, 0))
    out.putalpha(outline)
    return out


def save_charui(sources: dict[str, Path]) -> None:
    model_src = sources.get("character_model_master") or sources.get("character_model")
    combat_src = sources.get("character_model_combat") or model_src
    combat_model: Image.Image | None = None
    if combat_src:
        combat_model = open_rgba(combat_src)
        combat_canvas = fit_transparent(combat_model, CHARUI_SIZES["character_model"], pad=0.08)
        save_png(combat_canvas, IMAGES / "charui/character_model_nailong.png")
        save_rig_parts(combat_canvas)

    if model_src:
        model = open_rgba(model_src)
        for key, size in CHARUI_SIZES.items():
            if key == "character_model":
                continue
            out_name = f"{key}_nailong.png"
            if key == "char_select":
                out_name = "char_select_nailong.png"
            elif key == "char_select_locked":
                out_name = "char_select_nailong_locked.png"
            elif key == "character_icon":
                out_name = "character_icon_nailong.png"
            elif key == "map_marker":
                out_name = "map_marker_nailong.png"
            icon_model = combat_model if key == "character_icon" and combat_model else model
            save_png(fit_transparent(icon_model, size, pad=0.02), IMAGES / f"charui/{out_name}")

    for asset_id, src in sorted(sources.items()):
        if asset_id in DIRECT_UI_SIZES:
            save_direct_ui(asset_id, src)
        elif asset_id in ROOT_IMAGE_SIZES:
            save_root_image(asset_id, src)
    save_png(Image.new("RGBA", (128, 128), (0, 0, 0, 0)), IMAGES / "charui/energy_counter_blank.png")


def save_fallbacks() -> None:
    card = IMAGES / "card_portraits/nailong_strike.png"
    big_card = IMAGES / "card_portraits/big/nailong_strike.png"
    relic = IMAGES / "relics/nailong_belly.png"
    big_relic = IMAGES / "relics/big/nailong_belly.png"
    if card.exists():
        save_png(open_rgba(card), IMAGES / "card_portraits/card.png")
    if big_card.exists():
        save_png(open_rgba(big_card), IMAGES / "card_portraits/big/card.png")
    if relic.exists():
        small = open_rgba(relic)
        save_png(small, IMAGES / "relics/relic.png")
        save_png(make_outline(small), IMAGES / "relics/relic_outline.png")
    if big_relic.exists():
        save_png(open_rgba(big_relic), IMAGES / "relics/big/relic.png")


def main() -> None:
    sources = all_sources()
    for asset_id, src in sorted(sources.items()):
        if asset_id in CARD_IDS:
            save_card(asset_id, src)
        elif asset_id in POWER_IDS:
            save_power(asset_id, src)
        elif asset_id in RELIC_IDS:
            save_relic(asset_id, src)
    save_charui(sources)
    save_fallbacks()


if __name__ == "__main__":
    main()
