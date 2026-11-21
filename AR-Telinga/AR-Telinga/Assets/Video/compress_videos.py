"""
Kompres file video secara batch dengan FFmpeg.
Sumber : Assets/Video/Revisi/Video - mp4/
Output : Assets/Video/Revisi/Video - compress/ (struktur folder dipertahankan)

Script ini menampilkan proses per file langsung di terminal.
"""

from __future__ import annotations

import subprocess
import sys
from pathlib import Path

# ============================================================
# Konfigurasi path
# ============================================================
SCRIPT_DIR = Path(__file__).parent
INPUT_DIR = SCRIPT_DIR / "Revisi" / "Video - mp4"
OUTPUT_DIR = SCRIPT_DIR / "Revisi" / "Video - compress"

# Opsi kompresi:
# -c:v libx264  : codec video H.264
# -crf 28       : kompresi cukup agresif untuk ukuran lebih kecil
# -preset medium : keseimbangan ukuran dan kecepatan
# -c:a aac      : codec audio AAC
# -b:a 128k     : audio lebih kecil dari source umum
# -movflags +faststart : optimasi playback web
COMPRESS_OPTS = [
    "-c:v", "libx264",
    "-crf", "28",
    "-preset", "medium",
    "-c:a", "aac",
    "-b:a", "128k",
    "-movflags", "+faststart",
]


def check_ffmpeg() -> bool:
    """Periksa apakah FFmpeg tersedia di sistem."""
    try:
        result = subprocess.run(
            ["ffmpeg", "-version"],
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            check=False,
        )
        return result.returncode == 0
    except FileNotFoundError:
        return False


def run_ffmpeg(cmd: list[str]) -> int:
    """Jalankan FFmpeg dan tampilkan lognya langsung di terminal."""
    process = subprocess.Popen(
        cmd,
        stdout=subprocess.PIPE,
        stderr=subprocess.STDOUT,
        text=True,
        bufsize=1,
    )

    assert process.stdout is not None
    for raw_line in process.stdout:
        line = raw_line.rstrip()
        if line:
            print(f"      {line}")

    return process.wait()


def compress_file(src: Path, dst: Path) -> bool:
    """Kompres satu file video ke format MP4."""
    dst.parent.mkdir(parents=True, exist_ok=True)

    cmd = ["ffmpeg", "-y", "-i", str(src)] + COMPRESS_OPTS + [str(dst)]

    print(f"\n>>> Kompresi : {src.relative_to(INPUT_DIR)}")
    print(f"    Output   : {dst.relative_to(OUTPUT_DIR)}")

    return_code = run_ffmpeg(cmd)
    if return_code != 0:
        print("    [GAGAL] FFmpeg mengembalikan error")
        return False

    if dst.exists():
        size_mb = dst.stat().st_size / (1024 * 1024)
        print(f"    [OK] Ukuran output: {size_mb:.2f} MB")
    else:
        print("    [GAGAL] File output tidak ditemukan")
        return False

    return True


def main() -> None:
    print("=" * 60)
    print("  Kompres Video Batch")
    print("=" * 60)
    print(f"  Input  : {INPUT_DIR}")
    print(f"  Output : {OUTPUT_DIR}")
    print("=" * 60)

    if not check_ffmpeg():
        print("\n[ERROR] FFmpeg tidak ditemukan!")
        print("  Pastikan ffmpeg sudah terinstall dan tersedia di PATH.")
        sys.exit(1)

    if not INPUT_DIR.exists():
        print("\n[ERROR] Folder input tidak ditemukan.")
        sys.exit(1)

    video_files = sorted(
        [
            *INPUT_DIR.rglob("*.mp4"),
            *INPUT_DIR.rglob("*.m4v"),
            *INPUT_DIR.rglob("*.mov"),
        ]
    )

    # Hapus duplikat jika ada pola file yang terambil lebih dari sekali.
    video_files = sorted(set(video_files))

    if not video_files:
        print("\n[INFO] Tidak ada file video yang ditemukan di folder input.")
        sys.exit(0)

    OUTPUT_DIR.mkdir(parents=True, exist_ok=True)

    total = len(video_files)
    success = 0
    failed: list[Path] = []

    print(f"\nDitemukan {total} file video\n")

    for index, src in enumerate(video_files, start=1):
        rel_path = src.relative_to(INPUT_DIR)
        dst = OUTPUT_DIR / rel_path.with_suffix(".mp4")

        print(f"[{index}/{total}]", end="")
        ok = compress_file(src, dst)
        if ok:
            success += 1
        else:
            failed.append(src)

    print("\n" + "=" * 60)
    print(f"  Selesai : {success}/{total} file berhasil dikompres")
    if failed:
        print(f"\n  File yang GAGAL ({len(failed)}):")
        for item in failed:
            print(f"    - {item.relative_to(INPUT_DIR)}")
    print("=" * 60)


if __name__ == "__main__":
    main()