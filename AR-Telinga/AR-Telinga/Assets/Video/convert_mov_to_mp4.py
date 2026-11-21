"""
Konversi file video .mov ke .mp4
Sumber : Assets/Video/Revisi/Video - Mov/
Output  : Assets/Video/Revisi/Video - mp4/ (struktur folder dipertahankan)
Dependensi: FFmpeg harus sudah terinstall dan tersedia di PATH
"""

import os
import subprocess
import sys
from pathlib import Path

# ============================================================
# Konfigurasi path
# ============================================================
SCRIPT_DIR = Path(__file__).parent
INPUT_DIR  = SCRIPT_DIR / "Revisi" / "Video - Mov"
OUTPUT_DIR = SCRIPT_DIR / "Revisi" / "Video - mp4"

# Opsi cepat: cukup pindahkan stream ke MP4 bila codec sudah kompatibel.
COPY_OPTS = [
    "-c", "copy",
    "-movflags", "+faststart",
]

# Opsi fallback encoding jika stream copy gagal.
REENCODE_OPTS = [
    "-c:v", "libx264",
    "-crf", "18",
    "-preset", "fast",
    "-c:a", "aac",
    "-b:a", "192k",
    "-movflags", "+faststart",
]

# ============================================================
# Fungsi utama
# ============================================================

def check_ffmpeg() -> bool:
    """Periksa apakah FFmpeg tersedia di sistem."""
    try:
        result = subprocess.run(
            ["ffmpeg", "-version"],
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
        )
        return result.returncode == 0
    except FileNotFoundError:
        return False


def run_ffmpeg(cmd: list[str]) -> tuple[int, str]:
    """Jalankan FFmpeg dan tampilkan outputnya langsung di terminal."""
    process = subprocess.Popen(
        cmd,
        stdout=subprocess.PIPE,
        stderr=subprocess.STDOUT,
        text=True,
        bufsize=1,
    )

    last_line = ""
    assert process.stdout is not None
    for raw_line in process.stdout:
        line = raw_line.rstrip()
        if line:
            last_line = line
            print(f"      {line}")

    return process.wait(), last_line


def convert_file(src: Path, dst: Path) -> bool:
    """
    Konversi satu file .mov ke .mp4.
    Kembalikan True jika berhasil, False jika gagal.
    """
    dst.parent.mkdir(parents=True, exist_ok=True)

    copy_cmd = ["ffmpeg", "-y", "-i", str(src)] + COPY_OPTS + [str(dst)]
    reencode_cmd = ["ffmpeg", "-y", "-i", str(src)] + REENCODE_OPTS + [str(dst)]

    print(f"\n>>> Mengkonversi : {src.relative_to(INPUT_DIR)}")
    print(f"    Menyimpan ke : {dst.relative_to(OUTPUT_DIR)}")

    return_code, _ = run_ffmpeg(copy_cmd)

    if return_code != 0:
        print("    [INFO] Stream copy gagal, coba re-encode H.264/AAC...")
        return_code, _ = run_ffmpeg(reencode_cmd)

    if return_code != 0:
        print("    [GAGAL] Error FFmpeg")
        return False

    size_mb = dst.stat().st_size / (1024 * 1024)
    print(f"    [OK] Ukuran output: {size_mb:.2f} MB")
    return True


def main():
    print("=" * 60)
    print("  Konversi MOV → MP4")
    print("=" * 60)
    print(f"  Input  : {INPUT_DIR}")
    print(f"  Output : {OUTPUT_DIR}")
    print("=" * 60)

    OUTPUT_DIR.mkdir(parents=True, exist_ok=True)

    # Periksa FFmpeg
    if not check_ffmpeg():
        print("\n[ERROR] FFmpeg tidak ditemukan!")
        print("  Unduh dan install FFmpeg dari https://ffmpeg.org/download.html")
        print("  Pastikan folder bin FFmpeg sudah ditambahkan ke PATH sistem.")
        sys.exit(1)

    # Kumpulkan semua file .mov secara rekursif
    mov_files = sorted(INPUT_DIR.rglob("*.mov"))

    if not mov_files:
        print("\n[INFO] Tidak ada file .mov yang ditemukan di folder input.")
        sys.exit(0)

    total   = len(mov_files)
    success = 0
    failed  = []

    print(f"\nDitemukan {total} file .mov\n")

    for idx, src in enumerate(mov_files, start=1):
        # Buat path output dengan mempertahankan struktur sub-folder
        rel_path = src.relative_to(INPUT_DIR)
        dst = OUTPUT_DIR / rel_path.with_suffix(".mp4")

        print(f"[{idx}/{total}]", end="")
        ok = convert_file(src, dst)
        if ok:
            success += 1
        else:
            failed.append(src)

    # Ringkasan hasil
    print("\n" + "=" * 60)
    print(f"  Selesai : {success}/{total} file berhasil dikonversi")
    if failed:
        print(f"\n  File yang GAGAL ({len(failed)}):")
        for f in failed:
            print(f"    - {f.relative_to(INPUT_DIR)}")
    print("=" * 60)


if __name__ == "__main__":
    main()
