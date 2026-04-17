# UI/UX Rules — Phorg.Avalonia

## Layout
- Main padding: 16px. Section spacing: 12px. Inline spacing: 8px.
- Shared-row Grid (`ColumnDefinitions="Auto,*"`, explicit RowDefinitions): each row holds a button (left) and its paired content (right). This enforces vertical alignment structurally.
- Row order: Browse Source | path, Browse Dest | path, Scan | (empty), * fill | date groups, Copy | counter, (empty) | log.
- Wrap list content in ScrollViewer; keep the action bar always visible at bottom.

## Components
- Native file/folder pickers only — no custom path browsers.
- TextBox (read-only) for display fields; editable TextBox for user input.
- Buttons for all discrete actions; no context menus or dropdowns unless essential.

## State & Feedback
- Disable buttons during active operations (`IsEnabled="{Binding !IsBusy}"`).
- Enable action buttons only when required data is present (e.g. scan results before copy).
- Append progress to a log TextBox in real time — never batch results to end.
- Show numeric counters for completed items (e.g. CopiedCount).

## Interaction Flow
- Linear left-to-right, top-to-bottom flow: input → prepare → customize → execute.
- No modal dialogs or confirmation prompts — rely on disabled-state prevention.
- All operations happen in-place on the main window.

## Styling
- Use FluentTheme; no custom brushes or animations.
- Gray for secondary/metadata text (file counts, labels).
- Monospace font only for log output.
- No icons unless they clarify an otherwise ambiguous action.

## MVVM Conventions
- One ViewModel per View; state via ObservableProperty.
- Commands via RelayCommand with CanExecute derived from state booleans.
- No code-behind logic beyond InitializeComponent.
