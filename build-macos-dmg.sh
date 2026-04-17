#!/usr/bin/env bash
set -euo pipefail

APP_NAME="Phorg"
VERSION="1.0.0"
RID="osx-arm64"
DIST="dist"
PUBLISH_DIR="$DIST/publish"
APP_BUNDLE="$DIST/$APP_NAME.app"
DMG_OUT="$DIST/${APP_NAME}-${VERSION}-${RID}.dmg"

echo "==> Cleaning previous dist..."
rm -rf "$DIST"
mkdir -p "$DIST"

echo "==> Publishing $APP_NAME ($RID)..."
dotnet publish src/Phorg.Avalonia/Phorg.Avalonia.csproj \
    -c Release -r "$RID" --self-contained \
    -o "$PUBLISH_DIR"

echo "==> Creating .icns icon..."
ICONSET="$DIST/Phorg.iconset"
mkdir -p "$ICONSET"
SRC_ICON="assets/phorg-icon-1024x1024.png"
sips -z 16   16   "$SRC_ICON" --out "$ICONSET/icon_16x16.png"        > /dev/null
sips -z 32   32   "$SRC_ICON" --out "$ICONSET/icon_16x16@2x.png"     > /dev/null
sips -z 32   32   "$SRC_ICON" --out "$ICONSET/icon_32x32.png"        > /dev/null
sips -z 64   64   "$SRC_ICON" --out "$ICONSET/icon_32x32@2x.png"     > /dev/null
sips -z 128  128  "$SRC_ICON" --out "$ICONSET/icon_128x128.png"      > /dev/null
sips -z 256  256  "$SRC_ICON" --out "$ICONSET/icon_128x128@2x.png"   > /dev/null
sips -z 256  256  "$SRC_ICON" --out "$ICONSET/icon_256x256.png"      > /dev/null
sips -z 512  512  "$SRC_ICON" --out "$ICONSET/icon_256x256@2x.png"   > /dev/null
sips -z 512  512  "$SRC_ICON" --out "$ICONSET/icon_512x512.png"      > /dev/null
cp "$SRC_ICON"                      "$ICONSET/icon_512x512@2x.png"
iconutil -c icns "$ICONSET" -o "$DIST/Phorg.icns"
rm -rf "$ICONSET"

echo "==> Creating .app bundle..."
mkdir -p "$APP_BUNDLE/Contents/MacOS"
mkdir -p "$APP_BUNDLE/Contents/Resources"

cp -r "$PUBLISH_DIR/"* "$APP_BUNDLE/Contents/MacOS/"
chmod +x "$APP_BUNDLE/Contents/MacOS/$APP_NAME"
cp "$DIST/Phorg.icns" "$APP_BUNDLE/Contents/Resources/Phorg.icns"

cat > "$APP_BUNDLE/Contents/Info.plist" << PLIST
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleName</key>
    <string>$APP_NAME</string>
    <key>CFBundleDisplayName</key>
    <string>$APP_NAME</string>
    <key>CFBundleIdentifier</key>
    <string>com.zzacal.phorg</string>
    <key>CFBundleVersion</key>
    <string>$VERSION</string>
    <key>CFBundleShortVersionString</key>
    <string>$VERSION</string>
    <key>CFBundleExecutable</key>
    <string>$APP_NAME</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleIconFile</key>
    <string>Phorg</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>NSHumanReadableCopyright</key>
    <string>Copyright © 2025 zzacal</string>
</dict>
</plist>
PLIST

echo "==> Building DMG..."
create-dmg \
    --volname "$APP_NAME" \
    --window-size 520 300 \
    --icon-size 128 \
    --icon "$APP_NAME.app" 130 140 \
    --app-drop-link 390 140 \
    "$DMG_OUT" \
    "$APP_BUNDLE"

echo "==> Cleaning up..."
rm -rf "$PUBLISH_DIR" "$APP_BUNDLE" "$DIST/Phorg.icns"

echo ""
echo "Done: $DMG_OUT"
