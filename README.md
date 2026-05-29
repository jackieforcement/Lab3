# WpfLab3 — ToDo-приложение (WPF, MVVM)

Настольное приложение для ведения списка задач, написанное на WPF и .NET 8
по паттерну **MVVM**. Задачи хранятся в JSON-файле, поддерживаются фильтрация,
редактирование, удаление и отметка о выполнении.

## Возможности

- Добавление, редактирование и удаление задач.
- Отметка задачи как выполненной/невыполненной.
- Массовое удаление выбранных задач.
- Фильтрация списка: все / активные / выполненные.
- Сортировка по дате создания (новые сверху).
- Валидация ввода (название обязательно, ограничения по длине).
- Автоматическое сохранение изменений в JSON.
- Индикатор занятости и строка состояния.

## Технологии

- .NET 8 (`net8.0-windows`)
- WPF
- C# с включёнными `Nullable` и `ImplicitUsings`
- `System.Text.Json` для хранения данных
- NUnit для модульных тестов

## Архитектура

Проект следует паттерну MVVM с разделением на слои:

```
WpfLab3/
├── Models/            Модели данных (TodoTask, TaskFilter)
├── ViewModels/        Логика представления (MainViewModel, TaskEditViewModel, TaskItemViewModel)
├── Views/             XAML-окна (MainWindow, TaskEditWindow)
├── Services/          Репозиторий и диалоги (JsonTaskRepository, DialogService)
├── Mvvm/              Инфраструктура MVVM (ObservableObject, RelayCommand, AsyncRelayCommand, ObservableValidator)
├── Helpers/           Конвертеры значений для XAML
└── Resources/         Стили (Styles.xaml)

WpfLab3.Tests/         Модульные тесты (NUnit) с фейковыми реализациями сервисов
```

Ключевые элементы:

- **`ITaskRepository` / `JsonTaskRepository`** — асинхронное хранилище задач.
  Данные сохраняются в `%LocalAppData%\WpfLab3\tasks.json` через временный файл
  для атомарной записи.
- **`IDialogService` / `DialogService`** — абстракция над окнами и
  диалогами подтверждения, что делает ViewModel-слой тестируемым.
- **`AsyncRelayCommand` / `RelayCommand`** — реализации `ICommand` для
  привязки команд к UI.
- **`ObservableValidator`** — поддержка валидации через
  `System.ComponentModel.DataAnnotations`.

## Сборка и запуск

Требуется [.NET 8 SDK](https://dotnet.microsoft.com/download) и Windows.

```powershell
# Сборка
dotnet build WpfLab3/WpfLab3.csproj

# Запуск приложения
dotnet run --project WpfLab3/WpfLab3.csproj
```

## Тесты

```powershell
dotnet test WpfLab3.Tests/WpfLab3.Tests.csproj
```

Тесты используют фейковые реализации `ITaskRepository` и `IDialogService`,
поэтому выполняются без обращения к файловой системе и без отображения окон.

## Хранение данных

Задачи сохраняются в файле:

```
%LocalAppData%\WpfLab3\tasks.json
```

Файл создаётся автоматически при первом сохранении. Удаление файла сбрасывает
список задач.
