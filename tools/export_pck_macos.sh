#!/usr/bin/env bash
set -euo pipefail

project_dir="${1:-$PWD}"
preset_name="${2:-BasicExport}"
mod_id="${3:-PengfuNailongMod}"

cd "$project_dir"
project_dir="$(pwd -P)"

sdk_version="$(
  sed -n 's#.*Sdk="Godot.NET.Sdk/\([^"]*\)".*#\1#p' "$project_dir"/*.csproj 2>/dev/null |
    head -n 1
)"

if [[ -z "$sdk_version" ]]; then
  printf '无法从 .csproj 推断 Godot.NET.Sdk 版本。\n' >&2
  exit 1
fi

find_godot() {
  local candidate

  for candidate in \
    "${GODOT_BIN:-}" \
    "/Applications/Godot_mono.app/Contents/MacOS/Godot"; do
    if [[ -n "$candidate" && -x "$candidate" ]]; then
      printf '%s\n' "$candidate"
      return 0
    fi
  done

  return 1
}

ensure_godot() {
  local godot
  if godot="$(find_godot)"; then
    printf '%s\n' "$godot"
    return 0
  fi

  local cache_root="${GODOT_EXPORT_CACHE_DIR:-/tmp/sts2-godot-export}"
  local cache_dir="$cache_root/$sdk_version"
  local zip_path="$cache_root/Godot_v${sdk_version}-stable_mono_macos.universal.zip"
  local url="https://github.com/godotengine/godot/releases/download/${sdk_version}-stable/Godot_v${sdk_version}-stable_mono_macos.universal.zip"
  local cached_godot="$cache_dir/Godot_mono.app/Contents/MacOS/Godot"

  if [[ -x "$cached_godot" ]]; then
    printf '%s\n' "$cached_godot"
    return 0
  fi

  mkdir -p "$cache_root"
  if [[ ! -f "$zip_path" ]]; then
    curl -L --fail --progress-bar -o "$zip_path" "$url"
  fi

  rm -rf "$cache_dir"
  mkdir -p "$cache_dir"
  ditto -x -k "$zip_path" "$cache_dir"

  godot="$cached_godot"
  if [[ ! -x "$godot" ]]; then
    printf 'Godot 可执行文件不存在：%s\n' "$godot" >&2
    exit 1
  fi

  printf '%s\n' "$godot"
}

godot_bin="$(ensure_godot)"
output="$project_dir/$mod_id.pck"

rm -f "$output"
"$godot_bin" --headless --path "$project_dir" --export-pack "$preset_name" "$output"

if [[ ! -s "$output" ]]; then
  printf 'PCK 导出失败或为空：%s\n' "$output" >&2
  exit 1
fi

pck_file_list="$(mktemp)"
trap 'rm -f "$pck_file_list"' EXIT
"$project_dir/tools/list_pck_files.py" "$output" >"$pck_file_list"

if grep -Eq '(^|/)(PengfuNailongModCode|docs|tools|tmp|generated|bin|obj)/|\.cs$|\.csproj$|\.props$|\.sln$|\.dll$|\.pdb$|\.md$' "$pck_file_list"; then
  printf 'PCK 目录表包含了不应进入运行时包的源码、文档或构建目录。\n' >&2
  exit 1
fi

required_files=(
  "PengfuNailongMod/localization/zhs/cards.json"
  "PengfuNailongMod/localization/eng/cards.json"
  "PengfuNailongMod/images/card_portraits/nailong_strike.png.import"
  "PengfuNailongMod/images/charui/character_model_nailong.png.import"
  "PengfuNailongMod/audio/nailong/action_attack.ogg.import"
  "PengfuNailongMod/scenes/combat/nailong_rigged_visual.tscn.remap"
)

for required in "${required_files[@]}"; do
  if ! grep -Fxq "$required" "$pck_file_list"; then
    printf 'PCK 缺少运行时必需资源：%s\n' "$required" >&2
    exit 1
  fi
done

printf '已导出 %s\n' "$output"
shasum -a 256 "$output"
