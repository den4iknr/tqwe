# CS2 Demo Viewer — MAUI (Android / iOS)

Мобильное приложение для просмотра `.dem` файлов CS2 на телефоне.
Использует оригинальный `Cs2DemoViewer.Core.dll` для парсинга.

---

## Структура проекта

```
Cs2DemoViewer.Maui/
├── libs/                          ← СЮДА КОПИРУЕШЬ ВСЕ DLL
│   ├── Cs2DemoViewer.Core.dll
│   ├── DemoFile.dll
│   ├── DemoFile.Game.Cs.dll
│   ├── Google.Protobuf.dll
│   ├── protobuf-net.dll
│   ├── protobuf-net.Core.dll
│   └── Snappier.dll
│
├── Resources/Images/              ← СЮДА КЛАДЁШЬ PNG КАРТ
│   ├── map_de_dust2.png
│   ├── map_de_mirage.png
│   └── ... (опционально)
│
├── Models/          — модели данных
├── Services/        — DemoService, MapService
├── ViewModels/      — MainViewModel, DemoViewModel, RoundViewModel
├── Views/           — MainPage, DemoPage, RoundPage, MapCanvas
└── Converters/      — XAML конвертеры
```

---

## Как запустить

### Требования
- Visual Studio 2022 (17.8+) или JetBrains Rider
- MAUI Workload: `dotnet workload install maui`
- Android SDK (для Android) или Xcode (для iOS)
- .NET 9 SDK

### Шаги

1. **Скопируй DLL в папку `libs/`**
   ```
   Cs2DemoViewer_Core.dll     → libs/Cs2DemoViewer.Core.dll
   DemoFile.dll               → libs/DemoFile.dll
   DemoFile_Game_Cs.dll       → libs/DemoFile.Game.Cs.dll
   Google_Protobuf.dll        → libs/Google.Protobuf.dll
   protobuf-net.dll           → libs/protobuf-net.dll
   protobuf-net_Core.dll      → libs/protobuf-net.Core.dll
   Snappier.dll               → libs/Snappier.dll
   ```

2. **Открой проект в Visual Studio**
   ```
   Файл → Открыть → Cs2DemoViewer.Maui.csproj
   ```

3. **Добавь ресурсы приложения**
   Открой `App.xaml` и зарегистрируй конвертеры:
   ```xml
   <converters:StringToBoolConverter x:Key="StringToBoolConverter" />
   <converters:InverseBoolConverter  x:Key="InverseBoolConverter"  />
   ```

4. **Выбери платформу и устройство** (Android Emulator или физический телефон)

5. **Запусти** (F5 или кнопка ▶)

---

## Добавление карт (опционально)

Положи PNG-изображения радаров карт в `Resources/Images/`:
- Имя файла: `map_de_НАЗВАНИЕ.png`
- Например: `map_de_dust2.png`, `map_de_mirage.png`

Скачать радары можно из игры:
```
Steam/steamapps/common/Counter-Strike Global Offensive/game/csgo/resource/overviews/
```
Файлы `de_НАЗВАНИЕ_radar.png`

---

## Как пользоваться приложением

1. Открой `.dem` файл кнопкой **«Открыть .dem файл»**
2. Увидишь список раундов со счётом
3. Тапни на раунд — откроется карта с позициями игроков
4. Используй слайдер или кнопки ▶⏪⏩ для навигации по времени
5. Кнопка 📋 — список убийств в раунде

---

## Цветовая схема

| Цвет | Значение |
|------|---------|
| 🔵 Синий `#4a9eff` | Команда CT |
| 🟡 Жёлтый `#ffaa00` | Команда T |
| 🔴 Красный квадрат | Бомба |
| ⚪ Белый | Флэшка |
| 🔴 Красный | HE-граната |
| ⛽ Серый | Дымовая |
| 🟠 Оранжевый | Молотов / Инферно |
| 🟣 Фиолетовый | Декой |

---

## Архитектура

```
DemoParserService (Core.dll)
    ↓ ParseAsync(filePath)
DemoTimeline
    ↓ конвертация в DemoData
DemoViewModel → DemoPage (список раундов)
    ↓ выбор раунда
RoundViewModel → RoundPage (карта)
    ↓ слайдер FrameIndex
MapCanvas (GraphicsView) — рисует игроков и гранаты
```
