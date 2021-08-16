namespace HexView.Wpf
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;

    using BinaryTools.Elf.Io;

    /// <summary>
    /// Represents a control designed to display a classical hexadecimal viewer.
    /// </summary>
    [TemplatePart(Name = CanvasName, Type = typeof(Canvas))]
    [TemplatePart(Name = VerticalScrollBarName, Type = typeof(ScrollBar))]
    public class HexViewer : Control, INotifyPropertyChanged
    {
        /// <summary>
        /// Defines the address at which the data in the <see cref="DataSourceProperty"/> begins.
        /// </summary>
        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register(nameof(Address), typeof(ulong), typeof(HexViewer),
                new FrameworkPropertyMetadata(0UL, OnAddressChanged));

        /// <summary>
        /// Defines the brush used to display the addresses in the address section of the control.
        /// </summary>
        public static readonly DependencyProperty AddressBrushProperty =
            DependencyProperty.Register(nameof(AddressBrush), typeof(Brush), typeof(HexViewer),
                new FrameworkPropertyMetadata(SystemColors.HotTrackBrush, OnPropertyChangedInvalidateVisual));

        /// <summary>
        /// Defines the width of the addresses displayed in the address section of the control.
        /// </summary>
        public static readonly DependencyProperty AddressFormatProperty =
            DependencyProperty.Register(nameof(AddressFormat), typeof(AddressFormat), typeof(HexViewer),
                new FrameworkPropertyMetadata(AddressFormat.Address32, OnPropertyChangedInvalidateVisual));

        /// <summary>
        ///  Defines the brush used for alternating for text in alternating (odd numbered) columns in the data section of the control.
        /// </summary>
        public static readonly DependencyProperty AlternatingDataColumnTextBrushProperty =
            DependencyProperty.Register(nameof(AlternatingDataColumnTextBrush), typeof(Brush), typeof(HexViewer),
                new FrameworkPropertyMetadata(SystemColors.ActiveCaptionBrush, OnPropertyChangedInvalidateVisual));

        /// <summary>
        /// Defines the number of columns to display.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(nameof(Columns), typeof(int), typeof(HexViewer),
                new FrameworkPropertyMetadata(16, OnPropertyChangedInvalidateVisual, CoerceColumns));

        /// <summary>
        /// Defines the endianness used to interpret the data.
        /// </summary>
        public static readonly DependencyProperty EndiannessProperty =
            DependencyProperty.Register(nameof(Endianness), typeof(Endianness), typeof(HexViewer),
                new FrameworkPropertyMetadata(Endianness.BigEndian, OnPropertyChangedInvalidateVisual));

        /// <summary>
        /// Defines the format of the data to display.
        /// </summary>
        public static readonly DependencyProperty DataFormatProperty =
            DependencyProperty.Register(nameof(DataFormat), typeof(DataFormat), typeof(HexViewer),
                new FrameworkPropertyMetadata(DataFormat.Hexadecimal, OnPropertyChangedInvalidateVisual));

        /// <summary>
        /// Defines the signedness of the data to display.
        /// </summary>
        public static readonly DependencyProperty DataSignednessProperty =
            DependencyProperty.Register(nameof(DataSignedness), typeof(DataSignedness), typeof(HexViewer),
                new FrameworkPropertyMetadata(DataSignedness.Signed, OnPropertyChangedInvalidateVisual));

        /// <summary>
        /// Defines the data source which is used to read the data to display within this control.
        /// </summary>
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register(nameof(DataSource), typeof(BinaryReader), typeof(HexViewer),
                new FrameworkPropertyMetadata(OnDataSourceChanged));

        /// <summary>
        /// Defines the type of the data to display.
        /// </summary>
        public static readonly DependencyProperty DataTypeProperty =
            DependencyProperty.Register(nameof(DataType), typeof(DataType), typeof(HexViewer),
                new FrameworkPropertyMetadata(DataType.Integer, OnDataTypeChanged));

        /// <summary>
        /// Defines the width of the data to display.
        /// </summary>
        public static readonly DependencyProperty DataWidthProperty =
            DependencyProperty.Register(nameof(DataWidth), typeof(int), typeof(HexViewer),
                new FrameworkPropertyMetadata(1, OnDataWidthChanged, CoerceDataWidth), ValidateDataWidth);

        /// <summary>
        /// Defines the offset from the <see cref="DataSourceProperty"/> of the first visible data element being displayed.
        /// </summary>
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register(nameof(Offset), typeof(long), typeof(HexViewer),
                new FrameworkPropertyMetadata(0L, OnPropertyChangedInvalidateVisual, CoerceOffset));

        /// <summary>
        /// Defines the maximum number of columns, based on the size of the control, which can be displayed.
        /// </summary>
        public static readonly DependencyPropertyKey MaxVisibleColumnsPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(MaxVisibleColumns), typeof(int), typeof(HexViewer),
                new FrameworkPropertyMetadata(OnPropertyChangedInvalidateVisual, CoerceMaxVisibleColumns));

        /// <summary>
        /// Defines the maximum number of columns, based on the size of the control, which can be displayed.
        /// </summary>
        public static readonly DependencyProperty MaxVisibleColumnsProperty = MaxVisibleColumnsPropertyKey.DependencyProperty;

        /// <summary>
        /// Defines the maximum number of rows, based on the size of the control, which can be displayed.
        /// </summary>
        public static readonly DependencyPropertyKey MaxVisibleRowsPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(MaxVisibleRows), typeof(int), typeof(HexViewer),
                new FrameworkPropertyMetadata(OnPropertyChangedInvalidateVisual, CoerceMaxVisibleRows));

        /// <summary>
        /// Defines the maximum number of rows, based on the size of the control, which can be displayed.
        /// </summary>
        public static readonly DependencyProperty MaxVisibleRowsProperty = MaxVisibleRowsPropertyKey.DependencyProperty;

        /// <summary>
        /// Defines the brush used for selection fill.
        /// </summary>
        public static readonly DependencyProperty SelectionBrushProperty =
            DependencyProperty.Register(nameof(SelectionBrush), typeof(Brush), typeof(HexViewer),
                new FrameworkPropertyMetadata(SystemColors.HighlightBrush, OnPropertyChangedInvalidateVisual));

        /// <summary>
        /// Defines the brush used for selected text.
        /// </summary>
        public static readonly DependencyProperty SelectionTextBrushProperty =
            DependencyProperty.Register(nameof(SelectionTextBrush), typeof(Brush), typeof(HexViewer),
                new FrameworkPropertyMetadata(SystemColors.HighlightTextBrush, OnPropertyChangedInvalidateVisual));

        /// <summary>
        /// Defines the offset from <see cref="DataSourceProperty"/> of where the user selection has ended.
        /// </summary>
        public static readonly DependencyPropertyKey SelectionEndPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(SelectionEnd), typeof(long), typeof(HexViewer),
                new FrameworkPropertyMetadata(OnSelectionEndChanged, CoerceSelectionEnd));

        /// <summary>
        /// Defines the offset from <see cref="DataSourceProperty"/> of where the user selection has ended.
        /// </summary>
        public static readonly DependencyProperty SelectionEndProperty = SelectionEndPropertyKey.DependencyProperty;

        /// <summary>
        /// Defines the offset from <see cref="DataSourceProperty"/> of where the user selection has started.
        /// </summary>
        public static readonly DependencyPropertyKey SelectionStartPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(SelectionStart), typeof(long), typeof(HexViewer),
                new FrameworkPropertyMetadata(OnSelectionStartChanged, CoerceSelectionStart));

        /// <summary>
        /// Defines the offset from <see cref="DataSourceProperty"/> of where the user selection has started.
        /// </summary>
        public static readonly DependencyProperty SelectionStartProperty = SelectionStartPropertyKey.DependencyProperty;

        /// <summary>
        /// Determines whether to show the address section of the control.
        /// </summary>
        public static readonly DependencyProperty ShowAddressProperty =
            DependencyProperty.Register(nameof(ShowAddress), typeof(bool), typeof(HexViewer),
                new FrameworkPropertyMetadata(true, OnPropertyChangedInvalidateVisual));

        /// <summary>
        /// Determines whether to show the data section of the control.
        /// </summary>
        public static readonly DependencyProperty ShowDataProperty =
            DependencyProperty.Register(nameof(ShowData), typeof(bool), typeof(HexViewer),
                new FrameworkPropertyMetadata(true, OnPropertyChangedInvalidateVisual));

        /// <summary>
        /// Determines whether to show the text section of the control.
        /// </summary>
        public static readonly DependencyProperty ShowTextProperty =
            DependencyProperty.Register(nameof(ShowText), typeof(bool), typeof(HexViewer),
                new FrameworkPropertyMetadata(true, OnPropertyChangedInvalidateVisual));

        /// <summary>
        /// Defines the format of the text to display in the text section.
        /// </summary>
        public static readonly DependencyProperty TextFormatProperty =
            DependencyProperty.Register(nameof(TextFormat), typeof(TextFormat), typeof(HexViewer),
                new FrameworkPropertyMetadata(TextFormat.Ascii, OnPropertyChangedInvalidateVisual));

        private const string CanvasName = "PART_Canvas";
        private const string VerticalScrollBarName = "PART_VerticalScrollBar";

        private const int MaxColumns = 128;
        private const int MaxRows = 128;

        private const int CharsBetweenSections = 2;
        private const int CharsBetweenDataColumns = 1;
        private const int ScrollWheelScrollRows = 3;

        private FormattedText cachedFormattedChar;

        private Canvas canvas;

        private SelectionArea highlightBegin = SelectionArea.None;
        private SelectionArea highlightState = SelectionArea.None;

        private double lastVerticalScrollValue = 0;

        private ScrollBar verticalScrollBar;

        /// <summary>
        /// Initializes static members of the <see cref="HexViewer"/> class.
        /// </summary>
        static HexViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HexViewer), new FrameworkPropertyMetadata(typeof(HexViewer)));
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        private enum SelectionArea
        {
            None,
            Address,
            Data,
            Text,
        }

        /// <summary>
        /// Gets the <see cref="ApplicationCommands.Copy"/> routed command.
        /// </summary>
        public static RoutedUICommand CopyCommand => ApplicationCommands.Copy;

        /// <summary>
        /// Gets or sets the address at which the data in the <see cref="DataSource"/> begins.
        /// </summary>
        public ulong Address
        {
            get => (ulong)GetValue(AddressProperty);

            set => SetValue(AddressProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush used to display the addresses in the address section of the control.
        /// </summary>
        public Brush AddressBrush
        {
            get => (Brush)GetValue(AddressBrushProperty);

            set => SetValue(AddressBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush used for alternating for text in alternating (odd numbered) columns in the data section of the control.
        /// </summary>
        public Brush AlternatingDataColumnTextBrush
        {
            get => (Brush)GetValue(AlternatingDataColumnTextBrushProperty);

            set => SetValue(AlternatingDataColumnTextBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the number of columns to display.
        /// </summary>
        public int Columns
        {
            get => (int)GetValue(ColumnsProperty);

            set => SetValue(ColumnsProperty, value);
        }

        /// <summary>
        /// Gets or sets the endianness used to interpret the data.
        /// </summary>
        public Endianness Endianness
        {
            get => (Endianness)GetValue(EndiannessProperty);

            set => SetValue(EndiannessProperty, value);
        }

        /// <summary>
        /// Gets or sets the format of the data to display.
        /// </summary>
        public DataFormat DataFormat
        {
            get => (DataFormat)GetValue(DataFormatProperty);

            set => SetValue(DataFormatProperty, value);
        }

        /// <summary>
        /// Gets or sets the signedness of the data to display.
        /// </summary>
        public DataSignedness DataSignedness
        {
            get => (DataSignedness)GetValue(DataSignednessProperty);

            set => SetValue(DataSignednessProperty, value);
        }

        /// <summary>
        /// Gets or sets the data source which is used to read the data to display within this control.
        /// </summary>
        public BinaryReader DataSource
        {
            get => (BinaryReader)GetValue(DataSourceProperty);

            set => SetValue(DataSourceProperty, value);
        }

        /// <summary>
        /// Gets or sets the type of the data to display.
        /// </summary>
        public DataType DataType
        {
            get => (DataType)GetValue(DataTypeProperty);

            set => SetValue(DataTypeProperty, value);
        }

        /// <summary>
        /// Gets or sets the width of the data to display.
        /// </summary>
        public int DataWidth
        {
            get => (int)GetValue(DataWidthProperty);

            set => SetValue(DataWidthProperty, value);
        }

        /// <summary>
        /// Gets a value indicating whether the user has made any selection within the control.
        /// </summary>
        public bool IsSelectionActive => SelectionLength != 0;

        /// <summary>
        /// Gets the maximum number of columns, based on the size of the control, which can be displayed.
        /// </summary>
        public int MaxVisibleColumns
        {
            get => (int)GetValue(MaxVisibleColumnsProperty);

            private set => SetValue(MaxVisibleColumnsPropertyKey, value);
        }

        /// <summary>
        /// Gets the maximum number of rows, based on the size of the control, which can be displayed.
        /// </summary>
        public int MaxVisibleRows
        {
            get => (int)GetValue(MaxVisibleRowsProperty);

            private set => SetValue(MaxVisibleRowsPropertyKey, value);
        }

        /// <summary>
        /// Gets or sets the offset from the <see cref="DataSource"/> of the first visible data element being displayed.
        /// </summary>
        public long Offset
        {
            get => (long)GetValue(OffsetProperty);

            set => SetValue(OffsetProperty, value);
        }

        /// <summary>
        /// Gets lowest order address currently being selected.
        /// </summary>
        public ulong SelectedAddress => Address + (ulong)SelectedOffset;

        /// <summary>
        /// Gets the offset from <see cref="DataSource"/> of the <see cref="SelectedAddress"/>.
        /// </summary>
        public long SelectedOffset => Math.Min(SelectionStart, SelectionEnd);

        /// <summary>
        /// Gets or sets the brush used for selection fill.
        /// </summary>
        public Brush SelectionBrush
        {
            get => (Brush)GetValue(SelectionBrushProperty);

            set => SetValue(SelectionBrushProperty, value);
        }

        /// <summary>
        /// Gets the offset from <see cref="DataSource"/> of where the user selection has ended.
        /// </summary>
        public long SelectionEnd
        {
            get => (long)GetValue(SelectionEndProperty);

            private set => SetValue(SelectionEndPropertyKey, value);
        }

        /// <summary>
        /// Gets the number of bytes selected.
        /// </summary>
        public long SelectionLength
        {
            get
            {
                if (SelectionStart <= SelectionEnd)
                {
                    return SelectionEnd - SelectionStart;
                }
                else
                {
                    return SelectionStart - SelectionEnd + BytesPerColumn;
                }
            }
        }

        /// <summary>
        /// Gets the offset from <see cref="DataSource"/> of where the user selection has started.
        /// </summary>
        public long SelectionStart
        {
            get => (long)GetValue(SelectionStartProperty);

            private set => SetValue(SelectionStartPropertyKey, value);
        }

        /// <summary>
        /// Gets or sets the brush used for selected text.
        /// </summary>
        public Brush SelectionTextBrush
        {
            get => (Brush)GetValue(SelectionTextBrushProperty);

            set => SetValue(SelectionTextBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the address section of the control.
        /// </summary>
        public bool ShowAddress
        {
            get => (bool)GetValue(ShowAddressProperty);

            set => SetValue(ShowAddressProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the data section of the control.
        /// </summary>
        public bool ShowData
        {
            get => (bool)GetValue(ShowDataProperty);

            set => SetValue(ShowDataProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the text section of the control.
        /// </summary>
        public bool ShowText
        {
            get => (bool)GetValue(ShowTextProperty);

            set => SetValue(ShowTextProperty, value);
        }

        /// <summary>
        /// Gets or sets the width of the addresses displayed in the address section of the control.
        /// </summary>
        public AddressFormat AddressFormat
        {
            get => (AddressFormat)GetValue(AddressFormatProperty);

            set => SetValue(AddressFormatProperty, value);
        }

        /// <summary>
        /// Gets or sets the format of the text to display in the text section.
        /// </summary>
        public TextFormat TextFormat
        {
            get => (TextFormat)GetValue(TextFormatProperty);

            set => SetValue(TextFormatProperty, value);
        }

        private double SelectionBoxDataXPadding => cachedFormattedChar.Width / 4;

        private double SelectionBoxDataYPadding => 0;

        private double SelectionBoxTextXPadding => 0;

        private double SelectionBoxTextYPadding => 0;

        private int BytesPerColumn => DataWidth;

        private int BytesPerRow => DataWidth * Columns;

        /// <summary>
        /// Copies the current selection of the control to the <see cref="Clipboard"/>.
        /// </summary>
        public void Copy()
        {
            if (IsSelectionActive)
            {
                StringBuilder builder = new StringBuilder();

                long savedDataSourcePositionBeforeReadingData = DataSource.BaseStream.Position;

                DataSource.BaseStream.Position = Math.Min(SelectionStart, SelectionEnd);

                while (DataSource.BaseStream.Position < Math.Max(SelectionStart, SelectionEnd))
                {
                    var formattedData = ReadFormattedData();

                    builder.Append(formattedData);
                }

                DataSource.BaseStream.Position = savedDataSourcePositionBeforeReadingData;

                Clipboard.SetText(builder.ToString());
            }
        }

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // TODO: Validate FontFamily is a fixed width font
            // see https://social.msdn.microsoft.com/Forums/windows/en-US/5b582b96-ade5-4354-99cf-3fe64cc6b53b/determining-if-font-is-monospaced?forum=winforms
            canvas = GetTemplateChild(CanvasName) as Canvas;

            if (canvas != null)
            {
                CommandBindings.Add(new CommandBinding(
                    CopyCommand,
                    CopyExecuted,
                    CopyCanExecute));
            }
            else
            {
                throw new InvalidOperationException($"Could not find {CanvasName} template child.");
            }

            if (verticalScrollBar != null)
            {
                verticalScrollBar.Scroll -= OnVerticalScrollBarScroll;
            }

            verticalScrollBar = GetTemplateChild(VerticalScrollBarName) as ScrollBar;

            if (verticalScrollBar != null)
            {
                verticalScrollBar.Scroll += OnVerticalScrollBarScroll;
                verticalScrollBar.ValueChanged += OnVerticalScrollBarValueChanged;

                verticalScrollBar.Minimum = 0;
                verticalScrollBar.SmallChange = 1;
                verticalScrollBar.LargeChange = MaxVisibleRows;
            }
            else
            {
                throw new InvalidOperationException($"Could not find {VerticalScrollBarName} template child.");
            }
        }

        /// <summary>
        /// Scrolls the contents of the control to the specified offset.
        /// </summary>
        ///
        /// <param name="offset">
        /// The offset to scroll to.
        /// </param>
        public void ScrollToOffset(long offset)
        {
            long maxBytesDisplayed = BytesPerRow * MaxVisibleRows;

            if (Offset > offset)
            {
                // We need to scroll up
                Offset -= (((Offset - offset - 1) / BytesPerRow) + 1) * BytesPerRow;
            }

            if (Offset + maxBytesDisplayed <= offset)
            {
                // We need to scroll down
                Offset += (((offset - (Offset + maxBytesDisplayed)) / BytesPerRow) + 1) * BytesPerRow;
            }
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (Columns > 0 && MaxVisibleRows > 0)
            {
                switch (e.Key)
                {
                    case Key.A:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            SelectionStart = 0;
                            SelectionEnd = DataSource.BaseStream.Length;

                            e.Handled = true;
                        }

                        break;
                    }

                    case Key.C:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            e.Handled = true;
                        }

                        break;
                    }

                    case Key.Down:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                        {
                            SelectionEnd += BytesPerRow;
                        }
                        else
                        {
                            SelectionStart += BytesPerRow;
                            SelectionEnd = SelectionStart + BytesPerColumn;
                        }

                        ScrollToOffset(SelectionEnd - BytesPerColumn);

                        e.Handled = true;

                        break;
                    }

                    case Key.End:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            SelectionEnd = DataSource.BaseStream.Length;

                            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                            {
                                SelectionStart = SelectionEnd - BytesPerColumn;
                            }

                            ScrollToOffset(SelectionEnd - BytesPerColumn);
                        }
                        else
                        {
                            SelectionEnd += (Offset - SelectionEnd).Mod(BytesPerRow);

                            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                            {
                                SelectionStart = SelectionEnd - BytesPerColumn;
                            }

                            ScrollToOffset(SelectionEnd - BytesPerColumn);
                        }

                        e.Handled = true;

                        break;
                    }

                    case Key.Home:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            SelectionEnd = 0;

                            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                            {
                                SelectionStart = SelectionEnd;
                                SelectionEnd = SelectionStart + BytesPerColumn;
                            }

                            ScrollToOffset(SelectionEnd - BytesPerColumn);
                        }
                        else
                        {
                            // TODO: Because of the way we represent selection there is no way to distinguish at the
                            // moment whether the selection ends at the start of the current line or the end of the
                            // previous line. As such, when the Shift+End hotkey is used twice consecutively a whole
                            // new line above the current selection will be selected. This is undesirable behavior
                            // that deviates from the canonical semantics of Shift+End.
                            SelectionEnd -= (SelectionEnd - 1 - Offset).Mod(BytesPerRow) + 1;

                            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                            {
                                SelectionStart = SelectionEnd;
                                SelectionEnd = SelectionStart + BytesPerColumn;
                            }

                            ScrollToOffset(SelectionEnd - BytesPerColumn);
                        }

                        e.Handled = true;

                        break;
                    }

                    case Key.Left:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                        {
                            SelectionEnd -= BytesPerColumn;
                        }
                        else
                        {
                            SelectionStart -= BytesPerColumn;
                            SelectionEnd = SelectionStart + BytesPerColumn;
                        }

                        ScrollToOffset(SelectionEnd - BytesPerColumn);

                        e.Handled = true;

                        break;
                    }

                    case Key.PageDown:
                    {
                        bool isOffsetVisibleBeforeSelectionChange = IsOffsetVisible(SelectionEnd);

                        SelectionEnd += BytesPerRow * MaxVisibleRows;

                        if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                        {
                            SelectionStart = SelectionEnd - BytesPerColumn;
                        }

                        if (isOffsetVisibleBeforeSelectionChange)
                        {
                            ScrollToOffset(Offset + (BytesPerRow * MaxVisibleRows * 2) - BytesPerColumn);
                        }
                        else
                        {
                            ScrollToOffset(SelectionEnd - BytesPerColumn);
                        }

                        e.Handled = true;
                        break;
                    }

                    case Key.PageUp:
                    {
                        bool isOffsetVisibleBeforeSelectionChange = IsOffsetVisible(SelectionEnd);

                        SelectionEnd -= BytesPerRow * MaxVisibleRows;

                        if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                        {
                            SelectionStart = SelectionEnd - BytesPerColumn;
                            SelectionEnd = SelectionStart + BytesPerColumn;
                        }

                        if (isOffsetVisibleBeforeSelectionChange)
                        {
                            ScrollToOffset(Offset - (BytesPerRow * MaxVisibleRows));
                        }
                        else
                        {
                            ScrollToOffset(SelectionEnd - BytesPerColumn);
                        }

                        e.Handled = true;
                        break;
                    }

                    case Key.Right:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                        {
                            SelectionEnd += BytesPerColumn;
                        }
                        else
                        {
                            SelectionStart += BytesPerColumn;
                            SelectionEnd = SelectionStart + BytesPerColumn;
                        }

                        ScrollToOffset(SelectionEnd - BytesPerColumn);

                        e.Handled = true;
                        break;
                    }

                    case Key.Up:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                        {
                            SelectionEnd -= BytesPerRow;
                        }
                        else
                        {
                            SelectionStart -= BytesPerRow;
                            SelectionEnd = SelectionStart + BytesPerColumn;
                        }

                        ScrollToOffset(SelectionEnd - BytesPerColumn);

                        e.Handled = true;
                        break;
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                Point position = e.GetPosition(canvas);

                Point addressVerticalLinePoint0 = CalculateAddressVerticalLinePoint0();

                if (position.X < addressVerticalLinePoint0.X)
                {
                    highlightBegin = SelectionArea.Address;
                    highlightState = SelectionArea.Address;

                    SelectionStart = ConvertPositionToOffset(position);
                    SelectionEnd = SelectionStart + BytesPerRow;
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (highlightState == SelectionArea.None && CaptureMouse())
            {
                Point position = e.GetPosition(canvas);

                Point addressVerticalLinePoint0 = CalculateAddressVerticalLinePoint0();
                Point dataVerticalLinePoint0 = CalculateDataVerticalLinePoint0();
                Point textVerticalLinePoint0 = CalculateTextVerticalLinePoint0();

                if (position.X < addressVerticalLinePoint0.X)
                {
                    highlightBegin = SelectionArea.Address;
                    highlightState = SelectionArea.Address;
                }
                else if (position.X < dataVerticalLinePoint0.X)
                {
                    highlightBegin = SelectionArea.Data;
                    highlightState = SelectionArea.Data;
                }
                else if (position.X < textVerticalLinePoint0.X)
                {
                    highlightBegin = SelectionArea.Text;
                    highlightState = SelectionArea.Text;
                }

                if (highlightState != SelectionArea.None)
                {
                    SelectionStart = ConvertPositionToOffset(position);

                    SelectionEnd = SelectionStart + BytesPerColumn;
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            highlightState = SelectionArea.None;

            ReleaseMouseCapture();
        }

        /// <inheritdoc/>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            switch (highlightState)
            {
                case SelectionArea.Data:
                case SelectionArea.Text:
                {
                    Point position = e.GetPosition(canvas);

                    var currentMouseOverOffset = ConvertPositionToOffset(position);

                    if (currentMouseOverOffset >= SelectionStart)
                    {
                        SelectionEnd = currentMouseOverOffset + BytesPerColumn;
                    }
                    else
                    {
                        SelectionEnd = currentMouseOverOffset;
                    }

                    break;
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Delta < 0)
            {
                verticalScrollBar.Value += ScrollWheelScrollRows;

                OnVerticalScrollBarScroll(verticalScrollBar, new ScrollEventArgs(ScrollEventType.SmallIncrement, verticalScrollBar.Value));
            }
            else
            {
                verticalScrollBar.Value -= ScrollWheelScrollRows;

                OnVerticalScrollBarScroll(verticalScrollBar, new ScrollEventArgs(ScrollEventType.SmallDecrement, verticalScrollBar.Value));
            }
        }

        /// <inheritdoc/>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            UpdateState();

            canvas.Children.Clear();

            if (DataSource != null)
            {
                long savedDataSourcePosition = DataSource.BaseStream.Position;

                // Adjust the data source position based on the current offset
                DataSource.BaseStream.Position = Offset;

                DrawingVisual drawingVisual = new DrawingVisual();

                using (drawingContext = drawingVisual.RenderOpen())
                {
                    // Add a small padding of 1 pixel to not clip the selection box on the last row
                    var clipRect = new Rect(0, 0, canvas.ActualWidth, (MaxVisibleRows * cachedFormattedChar.Height) + 1.0);

                    // Clip the drawing to the bounds of the number of rows we can display to prevent the selection
                    // box from being drawn where there is no text. This can happen if the control size is changed
                    // while the selection remains active.
                    drawingContext.PushClip(new RectangleGeometry(clipRect));

                    var pen = new Pen(Foreground, 1.0);

                    double halfPenThickness = pen.Thickness / 2;

                    // Create guidelines to make sure our coordinate snap to device pixels
                    GuidelineSet guidelines = new GuidelineSet();

                    drawingContext.PushGuidelineSet(guidelines);

                    if (ShowAddress)
                    {
                        var addressVerticalLinePoint0 = CalculateAddressVerticalLinePoint0();
                        var addressVerticalLinePoint1 = CalculateAddressVerticalLinePoint1();

                        guidelines.GuidelinesX.Add(addressVerticalLinePoint0.X + halfPenThickness);
                        guidelines.GuidelinesX.Add(addressVerticalLinePoint1.X + halfPenThickness);
                        guidelines.GuidelinesY.Add(addressVerticalLinePoint0.Y + halfPenThickness);
                        guidelines.GuidelinesY.Add(addressVerticalLinePoint1.Y + halfPenThickness);

                        drawingContext.DrawLine(pen, addressVerticalLinePoint0, addressVerticalLinePoint1);
                    }

                    if (ShowData)
                    {
                        var dataVerticalLinePoint0 = CalculateDataVerticalLinePoint0();
                        var dataVerticalLinePoint1 = CalculateDataVerticalLinePoint1();

                        guidelines.GuidelinesX.Add(dataVerticalLinePoint0.X + halfPenThickness);
                        guidelines.GuidelinesX.Add(dataVerticalLinePoint1.X + halfPenThickness);
                        guidelines.GuidelinesY.Add(dataVerticalLinePoint0.Y + halfPenThickness);
                        guidelines.GuidelinesY.Add(dataVerticalLinePoint1.Y + halfPenThickness);

                        drawingContext.DrawLine(pen, dataVerticalLinePoint0, dataVerticalLinePoint1);

                        if (SelectionLength != 0 && MaxVisibleRows > 0 && Columns > 0)
                        {
                            Point selectionPoint0 = ConvertOffsetToPosition(SelectedOffset, SelectionArea.Data);
                            Point selectionPoint1 = ConvertOffsetToPosition(SelectedOffset + SelectionLength, SelectionArea.Data);

                            if (((SelectedOffset + SelectionLength - Offset) / BytesPerColumn) % Columns == 0)
                            {
                                // We're selecting the last column so the end point is the data vertical line (effectively)
                                selectionPoint1.X = dataVerticalLinePoint0.X - (CharsBetweenSections * cachedFormattedChar.Width);
                                selectionPoint1.Y -= cachedFormattedChar.Height;
                            }
                            else
                            {
                                selectionPoint1.X -= CharsBetweenDataColumns * cachedFormattedChar.Width;
                            }

                            DrawSelectionGeometry(drawingContext, SelectionBrush, pen, selectionPoint0, selectionPoint1, SelectionArea.Data);
                        }
                    }

                    if (ShowText)
                    {
                        var textVerticalLinePoint0 = CalculateTextVerticalLinePoint0();
                        var textVerticalLinePoint1 = CalculateTextVerticalLinePoint1();

                        guidelines.GuidelinesX.Add(textVerticalLinePoint0.X + halfPenThickness);
                        guidelines.GuidelinesX.Add(textVerticalLinePoint1.X + halfPenThickness);
                        guidelines.GuidelinesY.Add(textVerticalLinePoint0.Y + halfPenThickness);
                        guidelines.GuidelinesY.Add(textVerticalLinePoint1.Y + halfPenThickness);

                        drawingContext.DrawLine(pen, textVerticalLinePoint0, textVerticalLinePoint1);

                        if (SelectionLength != 0 && MaxVisibleRows > 0 && Columns > 0)
                        {
                            Point selectionPoint0 = ConvertOffsetToPosition(SelectedOffset, SelectionArea.Text);
                            Point selectionPoint1 = ConvertOffsetToPosition(SelectedOffset + SelectionLength, SelectionArea.Text);

                            if (((SelectedOffset + SelectionLength - Offset) / BytesPerColumn) % Columns == 0)
                            {
                                // We're selecting the last column so the end point is the text vertical line (effectively)
                                selectionPoint1.X = textVerticalLinePoint0.X - (CharsBetweenSections * cachedFormattedChar.Width);
                                selectionPoint1.Y -= cachedFormattedChar.Height;
                            }

                            DrawSelectionGeometry(drawingContext, SelectionBrush, pen, selectionPoint0, selectionPoint1, SelectionArea.Text);
                        }
                    }

                    Point origin = default;

                    Typeface cachedTypeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);

                    for (var row = 0; row < MaxVisibleRows; ++row)
                    {
                        if (ShowAddress)
                        {
                            if (DataSource.BaseStream.Position + BytesPerColumn <= DataSource.BaseStream.Length)
                            {
                                var textToFormat = GetFormattedAddressText(Address + (ulong)DataSource.BaseStream.Position);
                                var formattedText = new FormattedText(textToFormat, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, cachedTypeface, FontSize, AddressBrush, 1.0);
                                drawingContext.DrawText(formattedText, origin);

                                origin.X += (CalculateAddressColumnCharWidth() + CharsBetweenSections) * cachedFormattedChar.Width;
                            }
                        }

                        long savedDataSourcePositionBeforeReadingData = DataSource.BaseStream.Position;

                        if (ShowData)
                        {
                            origin.X += CharsBetweenSections * cachedFormattedChar.Width;

                            var cachedDataColumnCharWidth = CalculateDataColumnCharWidth();

                            // Needed to track text in alternating columns so we can use a different brush when drawing
                            var evenColumnBuilder = new StringBuilder(Columns * DataWidth);
                            var oddColumnBuilder = new StringBuilder(Columns * DataWidth);

                            var column = 0;

                            // Draw text up until selection start point
                            while (column < Columns)
                            {
                                if (DataSource.BaseStream.Position + BytesPerColumn <= DataSource.BaseStream.Length)
                                {
                                    if (DataSource.BaseStream.Position >= SelectedOffset)
                                    {
                                        break;
                                    }

                                    var textToFormat = ReadFormattedData();

                                    if (column % 2 == 0)
                                    {
                                        evenColumnBuilder.Append(textToFormat);
                                        evenColumnBuilder.Append(' ', CharsBetweenDataColumns);

                                        oddColumnBuilder.Append(' ', textToFormat.Length + CharsBetweenDataColumns);
                                    }
                                    else
                                    {
                                        oddColumnBuilder.Append(textToFormat);
                                        oddColumnBuilder.Append(' ', CharsBetweenDataColumns);

                                        evenColumnBuilder.Append(' ', textToFormat.Length + CharsBetweenDataColumns);
                                    }
                                }
                                else
                                {
                                    evenColumnBuilder.Append(' ', cachedDataColumnCharWidth + CharsBetweenDataColumns);
                                    oddColumnBuilder.Append(' ', cachedDataColumnCharWidth + CharsBetweenDataColumns);
                                }

                                ++column;
                            }

                            var evenFormattedText = new FormattedText(evenColumnBuilder.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, cachedTypeface, FontSize, Foreground, 1.0);
                            drawingContext.DrawText(evenFormattedText, origin);

                            var oddFormattedText = new FormattedText(oddColumnBuilder.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, cachedTypeface, FontSize, AlternatingDataColumnTextBrush, 1.0);
                            drawingContext.DrawText(oddFormattedText, origin);

                            origin.X += evenFormattedText.WidthIncludingTrailingWhitespace;

                            if (column < Columns)
                            {
                                // We'll reuse this builder for drawing selection text
                                evenColumnBuilder.Clear();

                                // Draw text starting from selection start point
                                while (column < Columns)
                                {
                                    if (DataSource.BaseStream.Position + BytesPerColumn <= DataSource.BaseStream.Length)
                                    {
                                        if (DataSource.BaseStream.Position >= SelectedOffset + SelectionLength)
                                        {
                                            break;
                                        }

                                        var textToFormat = ReadFormattedData();

                                        evenColumnBuilder.Append(textToFormat);
                                        evenColumnBuilder.Append(' ', CharsBetweenDataColumns);
                                    }
                                    else
                                    {
                                        evenColumnBuilder.Append(' ', cachedDataColumnCharWidth + CharsBetweenDataColumns);
                                    }

                                    ++column;
                                }

                                evenFormattedText = new FormattedText(evenColumnBuilder.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, cachedTypeface, FontSize, SelectionTextBrush, 1.0);
                                drawingContext.DrawText(evenFormattedText, origin);

                                origin.X += evenFormattedText.WidthIncludingTrailingWhitespace;

                                if (column < Columns)
                                {
                                    evenColumnBuilder.Clear();
                                    oddColumnBuilder.Clear();

                                    // Draw text after end of selection
                                    while (column < Columns)
                                    {
                                        if (DataSource.BaseStream.Position + BytesPerColumn <= DataSource.BaseStream.Length)
                                        {
                                            var textToFormat = ReadFormattedData();

                                            if (column % 2 == 0)
                                            {
                                                evenColumnBuilder.Append(textToFormat);
                                                evenColumnBuilder.Append(' ', CharsBetweenDataColumns);

                                                oddColumnBuilder.Append(' ', textToFormat.Length + CharsBetweenDataColumns);
                                            }
                                            else
                                            {
                                                oddColumnBuilder.Append(textToFormat);
                                                oddColumnBuilder.Append(' ', CharsBetweenDataColumns);

                                                evenColumnBuilder.Append(' ', textToFormat.Length + CharsBetweenDataColumns);
                                            }
                                        }
                                        else
                                        {
                                            evenColumnBuilder.Append(' ', cachedDataColumnCharWidth + CharsBetweenDataColumns);
                                            oddColumnBuilder.Append(' ', cachedDataColumnCharWidth + CharsBetweenDataColumns);
                                        }

                                        ++column;
                                    }

                                    evenFormattedText = new FormattedText(evenColumnBuilder.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, cachedTypeface, FontSize, Foreground, 1.0);
                                    drawingContext.DrawText(evenFormattedText, origin);

                                    oddFormattedText = new FormattedText(oddColumnBuilder.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, cachedTypeface, FontSize, AlternatingDataColumnTextBrush, 1.0);
                                    drawingContext.DrawText(oddFormattedText, origin);

                                    origin.X += evenFormattedText.WidthIncludingTrailingWhitespace;
                                }
                            }

                            // Compensate for the extra space added at the end of the builder
                            origin.X += (CharsBetweenSections - CharsBetweenDataColumns) * cachedFormattedChar.Width;
                        }

                        if (ShowText)
                        {
                            origin.X += CharsBetweenSections * cachedFormattedChar.Width;

                            if (ShowData)
                            {
                                // Reset the stream to read one byte at a time
                                DataSource.BaseStream.Position = savedDataSourcePositionBeforeReadingData;
                            }

                            var builder = new StringBuilder(Columns * DataWidth);

                            var column = 0;

                            // Draw text up until selection start point
                            while (column < Columns)
                            {
                                if (DataSource.BaseStream.Position + BytesPerColumn <= DataSource.BaseStream.Length)
                                {
                                    if (DataSource.BaseStream.Position >= SelectedOffset)
                                    {
                                        break;
                                    }

                                    var textToFormat = ReadFormattedText();
                                    builder.Append(textToFormat);
                                }

                                ++column;
                            }

                            var formattedText = new FormattedText(builder.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, cachedTypeface, FontSize, Foreground, 1.0);
                            drawingContext.DrawText(formattedText, origin);

                            if (column < Columns)
                            {
                                origin.X += formattedText.WidthIncludingTrailingWhitespace;

                                builder.Clear();

                                // Draw text starting from selection start point
                                while (column < Columns)
                                {
                                    if (DataSource.BaseStream.Position + BytesPerColumn <= DataSource.BaseStream.Length)
                                    {
                                        if (DataSource.BaseStream.Position >= SelectedOffset + SelectionLength)
                                        {
                                            break;
                                        }

                                        var textToFormat = ReadFormattedText();
                                        builder.Append(textToFormat);
                                    }

                                    ++column;
                                }

                                formattedText = new FormattedText(builder.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, cachedTypeface, FontSize, SelectionTextBrush, 1.0);
                                drawingContext.DrawText(formattedText, origin);

                                if (column < Columns)
                                {
                                    origin.X += formattedText.WidthIncludingTrailingWhitespace;

                                    builder.Clear();

                                    // Draw text after end of selection
                                    while (column < Columns)
                                    {
                                        if (DataSource.BaseStream.Position + BytesPerColumn <= DataSource.BaseStream.Length)
                                        {
                                            var textToFormat = ReadFormattedText();
                                            builder.Append(textToFormat);
                                        }

                                        ++column;
                                    }

                                    formattedText = new FormattedText(builder.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, cachedTypeface, FontSize, Foreground, 1.0);
                                    drawingContext.DrawText(formattedText, origin);
                                }
                            }
                        }

                        origin.X = 0;
                        origin.Y += cachedFormattedChar.Height;
                    }

                    DataSource.BaseStream.Position = savedDataSourcePosition;

                    drawingContext.Pop();
                    drawingContext.Pop();
                }

                var visualHost = new CanvasVisualHost
                {
                    Visual = drawingVisual,
                    IsHitTestVisible = false,
                };

                canvas.Children.Add(visualHost);
            }
        }

        private static void OnPropertyChangedInvalidateVisual(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var hexViewer = (HexViewer)d;

            hexViewer.InvalidateVisual();
        }

        private static void OnSelectionEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var hexViewer = (HexViewer)d;

            hexViewer.InvalidateVisual();
            hexViewer.OnPropertyChanged(nameof(SelectionEnd));
            hexViewer.OnPropertyChanged(nameof(SelectionLength));
            hexViewer.OnPropertyChanged(nameof(SelectedOffset));
            hexViewer.OnPropertyChanged(nameof(SelectedAddress));
            hexViewer.OnPropertyChanged(nameof(IsSelectionActive));
        }

        private static void OnSelectionStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var hexViewer = (HexViewer)d;

            hexViewer.InvalidateVisual();
            hexViewer.OnPropertyChanged(nameof(SelectionStart));
            hexViewer.OnPropertyChanged(nameof(SelectionLength));
            hexViewer.OnPropertyChanged(nameof(SelectedOffset));
            hexViewer.OnPropertyChanged(nameof(SelectedAddress));
            hexViewer.OnPropertyChanged(nameof(IsSelectionActive));
        }

        private static object CoerceColumns(DependencyObject d, object value)
        {
            var hexViewer = (HexViewer)d;

            if (hexViewer.MaxVisibleColumns == 0)
            {
                return (int)value;
            }
            else
            {
                return Math.Min((int)value, hexViewer.MaxVisibleColumns);
            }
        }

        private static object CoerceMaxVisibleColumns(DependencyObject d, object value)
        {
            return Math.Min((int)value, MaxColumns);
        }

        private static object CoerceMaxVisibleRows(DependencyObject d, object value)
        {
            return Math.Min((int)value, MaxRows);
        }

        private static object CoerceSelectionStart(DependencyObject d, object value)
        {
            var hexViewer = (HexViewer)d;

            if (hexViewer.DataSource != null)
            {
                long selectionStart = (long)value;

                // Selection offset cannot start in the middle of the data width
                selectionStart -= selectionStart % hexViewer.BytesPerColumn;

                // Selection start cannot be at the end of the stream so adjust by data width number of bytes
                value = selectionStart.Clamp(0, (hexViewer.DataSource.BaseStream.Length / hexViewer.BytesPerColumn * hexViewer.BytesPerColumn) - hexViewer.BytesPerColumn);
            }
            else
            {
                value = 0L;
            }

            return value;
        }

        private static object CoerceSelectionEnd(DependencyObject d, object value)
        {
            var hexViewer = (HexViewer)d;

            if (hexViewer.DataSource != null)
            {
                long selectionEnd = (long)value;

                // Selection offset cannot start in the middle of the data width
                selectionEnd -= selectionEnd % hexViewer.BytesPerColumn;

                // Unlike selection start the selection end can be at the end of the stream
                value = selectionEnd.Clamp(0, hexViewer.DataSource.BaseStream.Length / hexViewer.BytesPerColumn * hexViewer.BytesPerColumn);
            }
            else
            {
                value = 0L;
            }

            return value;
        }

        private static object CoerceDataWidth(DependencyObject d, object value)
        {
            var hexViewer = (HexViewer)d;

            if (hexViewer.DataType == DataType.FloatingPoint && (int)value < 4)
            {
                value = 4;
            }

            return value;
        }

        private static object CoerceOffset(DependencyObject d, object value)
        {
            var hexViewer = (HexViewer)d;

            if (hexViewer.DataSource != null)
            {
                long offset = (long)value;

                value = offset.Clamp(0, hexViewer.DataSource.BaseStream.Length);
            }
            else
            {
                value = 0L;
            }

            return value;
        }

        private static bool ValidateDataWidth(object value)
        {
            bool result = false;

            switch ((int)value)
            {
                case 1:
                case 2:
                case 4:
                case 8:
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        private static void OnAddressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var hexViewer = (HexViewer)d;

            hexViewer.SelectionStart = 0;
            hexViewer.SelectionEnd = 0;

            hexViewer.InvalidateVisual();
            hexViewer.OnPropertyChanged(nameof(Address));
            hexViewer.OnPropertyChanged(nameof(SelectedAddress));
        }

        private static void OnDataTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var hexViewer = (HexViewer)d;

            hexViewer.CoerceValue(DataWidthProperty);

            hexViewer.InvalidateVisual();
        }

        private static void OnDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var hexViewer = (HexViewer)d;

            hexViewer.Offset = 0;
            hexViewer.SelectionStart = 0;
            hexViewer.SelectionEnd = 0;

            hexViewer.InvalidateVisual();
        }

        private static void OnDataWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var hexViewer = (HexViewer)d;

            hexViewer.SelectionStart = 0;
            hexViewer.SelectionEnd = 0;

            hexViewer.InvalidateVisual();
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string ReadFormattedText()
        {
            StringBuilder builder = new StringBuilder(DataWidth);

            switch (TextFormat)
            {
                case TextFormat.Ascii:
                {
                    for (var k = 0; k < DataWidth; ++k)
                    {
                        byte value = DataSource.ReadByte();

                        if (value > 31 && value < 127)
                        {
                            builder.Append(Convert.ToChar(value));
                        }
                        else
                        {
                            builder.Append('.');
                        }
                    }

                    break;
                }

                default:
                {
                    throw new InvalidOperationException($"Invalid {nameof(TextFormat)} value.");
                }
            }

            return builder.ToString();
        }

        private string ReadFormattedData()
        {
            string result;

            switch (DataType)
            {
                case DataType.Integer:
                {
                    switch (DataFormat)
                    {
                        case DataFormat.Decimal:
                        {
                            switch (DataSignedness)
                            {
                                case DataSignedness.Signed:
                                {
                                    switch (DataWidth)
                                    {
                                        case 1:
                                        {
                                            result = $"{DataSource.ReadSByte():+#;-#;0}".PadLeft(4);
                                            break;
                                        }

                                        case 2:
                                        {
                                            result = $"{EndianBitConverter.Convert(DataSource.ReadInt16(), Endianness):+#;-#;0}".PadLeft(6);
                                            break;
                                        }

                                        case 4:
                                        {
                                            result = $"{EndianBitConverter.Convert(DataSource.ReadInt32(), Endianness):+#;-#;0}".PadLeft(11);
                                            break;
                                        }

                                        case 8:
                                        {
                                            result = $"{EndianBitConverter.Convert(DataSource.ReadInt64(), Endianness):+#;-#;0}".PadLeft(21);
                                            break;
                                        }

                                        default:
                                        {
                                            throw new InvalidOperationException($"Invalid {nameof(DataWidth)} value.");
                                        }
                                    }

                                    break;
                                }

                                case DataSignedness.Unsigned:
                                {
                                    switch (DataWidth)
                                    {
                                        case 1:
                                        {
                                            result = $"{DataSource.ReadByte()}".PadLeft(3);
                                            break;
                                        }

                                        case 2:
                                        {
                                            result = $"{EndianBitConverter.Convert(DataSource.ReadUInt16(), Endianness)}".PadLeft(5);
                                            break;
                                        }

                                        case 4:
                                        {
                                            result = $"{EndianBitConverter.Convert(DataSource.ReadUInt32(), Endianness)}".PadLeft(10);
                                            break;
                                        }

                                        case 8:
                                        {
                                            result = $"{EndianBitConverter.Convert(DataSource.ReadUInt64(), Endianness)}".PadLeft(20);
                                            break;
                                        }

                                        default:
                                        {
                                            throw new InvalidOperationException($"Invalid {nameof(DataWidth)} value.");
                                        }
                                    }

                                    break;
                                }

                                default:
                                {
                                    throw new InvalidOperationException($"Invalid {nameof(DataType)} value.");
                                }
                            }

                            break;
                        }

                        case DataFormat.Hexadecimal:
                        {
                            switch (DataWidth)
                            {
                                case 1:
                                {
                                    result = $"{DataSource.ReadByte(),0:X2}";
                                    break;
                                }

                                case 2:
                                {
                                    result = $"{EndianBitConverter.Convert(DataSource.ReadUInt16(), Endianness),0:X4}";
                                    break;
                                }

                                case 4:
                                {
                                    result = $"{EndianBitConverter.Convert(DataSource.ReadUInt32(), Endianness),0:X8}";
                                    break;
                                }

                                case 8:
                                {
                                    result = $"{EndianBitConverter.Convert(DataSource.ReadUInt64(), Endianness),0:X16}";
                                    break;
                                }

                                default:
                                {
                                    throw new InvalidOperationException($"Invalid {nameof(DataWidth)} value.");
                                }
                            }

                            break;
                        }

                        default:
                        {
                            throw new InvalidOperationException($"Invalid {nameof(DataFormat)} value.");
                        }
                    }

                    break;
                }

                case DataType.FloatingPoint:
                {
                    switch (DataWidth)
                    {
                        case 4:
                        {
                            var bytes = BitConverter.GetBytes(EndianBitConverter.Convert(DataSource.ReadUInt32(), Endianness));
                            var value = BitConverter.ToSingle(bytes, 0);
                            result = $"{value:E08}".PadLeft(16);
                            break;
                        }

                        case 8:
                        {
                            var bytes = BitConverter.GetBytes(EndianBitConverter.Convert(DataSource.ReadUInt64(), Endianness));
                            var value = BitConverter.ToSingle(bytes, 0);
                            result = $"{value:E16}".PadLeft(24);
                            break;
                        }

                        default:
                        {
                            throw new InvalidOperationException($"Invalid {nameof(DataWidth)} value.");
                        }
                    }

                    break;
                }

                default:
                {
                    throw new InvalidOperationException($"Invalid {nameof(DataType)} value.");
                }
            }

            return result;
        }

        private void CopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Copy();
        }

        private void CopyCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsSelectionActive;
        }

        private void OnVerticalScrollBarValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lastVerticalScrollValue = e.OldValue;
        }

        private void OnVerticalScrollBarScroll(object sender, ScrollEventArgs e)
        {
            long valueDelta = (long)(e.NewValue - lastVerticalScrollValue);

            long newOffset = Offset + (valueDelta * BytesPerRow);

            if (newOffset < 0)
            {
                newOffset = 0;
            }

            Offset = newOffset;

            InvalidateVisual();
        }

        private string GetFormattedAddressText(ulong address)
        {
            string formattedAddressText;

            switch (AddressFormat)
            {
                case AddressFormat.Address16:
                {
                    formattedAddressText = $"{address & 0xFFFF,0:X4}";
                    break;
                }

                case AddressFormat.Address24:
                {
                    formattedAddressText = $"{(address >> 16) & 0xFF,0:X2}:{address & 0xFFFF,0:X4}";
                    break;
                }

                case AddressFormat.Address32:
                {
                    formattedAddressText = $"{(address >> 16) & 0xFFFF,0:X4}:{address & 0xFFFF,0:X4}";
                    break;
                }

                case AddressFormat.Address48:
                {
                    formattedAddressText = $"{(address >> 32) & 0xFF,0:X4}:{address & 0xFFFFFFFF,0:X8}";
                    break;
                }

                case AddressFormat.Address64:
                {
                    formattedAddressText = $"{address >> 32,0:X8}:{address & 0xFFFFFFFF,0:X8}";
                    break;
                }

                default:
                {
                    throw new InvalidOperationException($"Invalid {nameof(AddressFormat)} value.");
                }
            }

            return formattedAddressText;
        }

        private int CalculateAddressColumnCharWidth()
        {
            int addressColumnCharWidth;

            switch (AddressFormat)
            {
                case AddressFormat.Address16:
                {
                    addressColumnCharWidth = 4;
                    break;
                }

                case AddressFormat.Address24:
                {
                    addressColumnCharWidth = 7;
                    break;
                }

                case AddressFormat.Address32:
                {
                    addressColumnCharWidth = 9;
                    break;
                }

                case AddressFormat.Address48:
                {
                    addressColumnCharWidth = 13;
                    break;
                }

                case AddressFormat.Address64:
                {
                    addressColumnCharWidth = 17;
                    break;
                }

                default:
                {
                    throw new InvalidOperationException($"Invalid {nameof(AddressFormat)} value.");
                }
            }

            return addressColumnCharWidth;
        }

        private int CalculateDataColumnCharWidth()
        {
            int dataColumnCharWidth;

            switch (DataType)
            {
                case DataType.Integer:
                {
                    switch (DataFormat)
                    {
                        case DataFormat.Decimal:
                        {
                            switch (DataSignedness)
                            {
                                case DataSignedness.Signed:
                                {
                                    switch (DataWidth)
                                    {
                                        case 1:
                                        {
                                            dataColumnCharWidth = 4;
                                            break;
                                        }

                                        case 2:
                                        {
                                            dataColumnCharWidth = 6;
                                            break;
                                        }

                                        case 4:
                                        {
                                            dataColumnCharWidth = 11;
                                            break;
                                        }

                                        case 8:
                                        {
                                            dataColumnCharWidth = 21;
                                            break;
                                        }

                                        default:
                                        {
                                            throw new InvalidOperationException($"Invalid {nameof(DataWidth)} value.");
                                        }
                                    }
                                }

                                break;

                                case DataSignedness.Unsigned:
                                {
                                    switch (DataWidth)
                                    {
                                        case 1:
                                        {
                                            dataColumnCharWidth = 3;
                                            break;
                                        }

                                        case 2:
                                        {
                                            dataColumnCharWidth = 5;
                                            break;
                                        }

                                        case 4:
                                        {
                                            dataColumnCharWidth = 10;
                                            break;
                                        }

                                        case 8:
                                        {
                                            dataColumnCharWidth = 20;
                                            break;
                                        }

                                        default:
                                        {
                                            throw new InvalidOperationException($"Invalid {nameof(DataWidth)} value.");
                                        }
                                    }
                                }

                                break;

                                default:
                                {
                                    throw new InvalidOperationException($"Invalid {nameof(DataType)} value.");
                                }
                            }
                        }

                        break;

                        case DataFormat.Hexadecimal:
                        {
                            switch (DataWidth)
                            {
                                case 1:
                                case 2:
                                case 4:
                                case 8:
                                {
                                    dataColumnCharWidth = 2 * DataWidth;
                                    break;
                                }

                                default:
                                {
                                    throw new InvalidOperationException($"Invalid {nameof(DataWidth)} value.");
                                }
                            }

                            break;
                        }

                        default:
                        {
                            throw new InvalidOperationException($"Invalid {nameof(DataFormat)} value.");
                        }
                    }
                }

                break;

                case DataType.FloatingPoint:
                {
                    switch (DataWidth)
                    {
                        case 4:
                        {
                            dataColumnCharWidth = 16;
                            break;
                        }

                        case 8:
                        {
                            dataColumnCharWidth = 24;
                            break;
                        }

                        default:
                        {
                            throw new InvalidOperationException($"Invalid {nameof(DataWidth)} value.");
                        }
                    }
                }

                break;

                default:
                {
                    throw new InvalidOperationException($"Invalid {nameof(DataType)} value.");
                }
            }

            return dataColumnCharWidth;
        }

        private Point CalculateAddressVerticalLinePoint0()
        {
            Point point1 = default;

            if (ShowAddress)
            {
                point1.X = (CalculateAddressColumnCharWidth() + CharsBetweenSections) * cachedFormattedChar.Width;
            }

            return point1;
        }

        private Point CalculateAddressVerticalLinePoint1()
        {
            Point point2 = default;

            if (ShowAddress)
            {
                point2.X = (CalculateAddressColumnCharWidth() + CharsBetweenSections) * cachedFormattedChar.Width;
            }

            point2.Y = Math.Min(cachedFormattedChar.Height * MaxVisibleRows, canvas.ActualHeight);

            return point2;
        }

        private Point CalculateDataVerticalLinePoint0()
        {
            Point point1 = CalculateAddressVerticalLinePoint0();

            if (ShowData)
            {
                point1.X += (CharsBetweenSections + ((CalculateDataColumnCharWidth() + CharsBetweenDataColumns) * Columns) - CharsBetweenDataColumns + CharsBetweenSections) * cachedFormattedChar.Width;
            }

            return point1;
        }

        private Point CalculateDataVerticalLinePoint1()
        {
            Point point2 = CalculateAddressVerticalLinePoint1();

            if (ShowData)
            {
                point2.X += (CharsBetweenSections + ((CalculateDataColumnCharWidth() + CharsBetweenDataColumns) * Columns) - CharsBetweenDataColumns + CharsBetweenSections) * cachedFormattedChar.Width;
            }

            return point2;
        }

        private int CalculateTextColumnCharWidth()
        {
            return BytesPerColumn;
        }

        private Point CalculateTextVerticalLinePoint0()
        {
            Point point1 = CalculateDataVerticalLinePoint0();

            if (ShowText)
            {
                point1.X += (CharsBetweenSections + (CalculateTextColumnCharWidth() * Columns) + CharsBetweenSections) * cachedFormattedChar.Width;
            }

            return point1;
        }

        private Point CalculateTextVerticalLinePoint1()
        {
            Point point2 = CalculateDataVerticalLinePoint1();

            if (ShowText)
            {
                point2.X += (CharsBetweenSections + (CalculateTextColumnCharWidth() * Columns) + CharsBetweenSections) * cachedFormattedChar.Width;
            }

            return point2;
        }

        private void DrawSelectionGeometry(DrawingContext drawingContext, Brush brush, Pen pen, Point point0, Point point1, SelectionArea relativeTo)
        {
            if ((long)point0.Y > (long)point1.Y)
            {
                throw new ArgumentException($"{nameof(point0)}.Y > {nameof(point1)}.Y", nameof(point0));
            }

            Point lhsVerticalLinePoint0;
            Point rhsVerticalLinePoint0;

            double selectionBoxXPadding;
            double selectionBoxYPadding;

            switch (relativeTo)
            {
                case SelectionArea.Data:
                {
                    lhsVerticalLinePoint0 = CalculateAddressVerticalLinePoint0();
                    rhsVerticalLinePoint0 = CalculateDataVerticalLinePoint0();

                    selectionBoxXPadding = SelectionBoxDataXPadding;
                    selectionBoxYPadding = SelectionBoxDataYPadding;
                }

                break;

                case SelectionArea.Text:
                {
                    lhsVerticalLinePoint0 = CalculateDataVerticalLinePoint0();
                    rhsVerticalLinePoint0 = CalculateTextVerticalLinePoint0();

                    selectionBoxXPadding = SelectionBoxTextXPadding;
                    selectionBoxYPadding = SelectionBoxTextYPadding;
                }

                break;

                default:
                {
                    throw new ArgumentException($"Invalid relative area {relativeTo}", nameof(relativeTo));
                }
            }

            // Create guidelines to make sure our coordinate snap to device pixels
            GuidelineSet guidelines = new GuidelineSet();

            drawingContext.PushGuidelineSet(guidelines);

            double halfPenThickness = pen.Thickness / 2;

            PathGeometry geometry = new PathGeometry();

            point0.X -= selectionBoxXPadding;
            point1.X += selectionBoxXPadding;
            point0.Y -= selectionBoxYPadding;
            point1.Y += selectionBoxYPadding;

            PathFigure figure = new PathFigure
            {
                StartPoint = point0,
                IsClosed = true,
            };

            if ((long)point0.X < (long)point1.X)
            {
                if ((long)point0.Y < (long)point1.Y)
                {
                    // +---------------------------+
                    // |                           |
                    // |             0-------------2
                    // |             |             |
                    // 6-------------7     1-------3
                    // |                   |       |
                    // 5-------------------4       |
                    // |                           |
                    // |                           |
                    // |                           |
                    // +---------------------------+
                    Point point2 = new Point(rhsVerticalLinePoint0.X - (CharsBetweenSections * cachedFormattedChar.Width) + selectionBoxXPadding, point0.Y);
                    Point point3 = new Point(rhsVerticalLinePoint0.X - (CharsBetweenSections * cachedFormattedChar.Width) + selectionBoxXPadding, point1.Y);
                    Point point4 = new Point(point1.X, point1.Y + cachedFormattedChar.Height);
                    Point point5 = new Point(lhsVerticalLinePoint0.X + (CharsBetweenSections * cachedFormattedChar.Width) - selectionBoxXPadding, point1.Y + cachedFormattedChar.Height);
                    Point point6 = new Point(lhsVerticalLinePoint0.X + (CharsBetweenSections * cachedFormattedChar.Width) - selectionBoxXPadding, point0.Y + cachedFormattedChar.Height);
                    Point point7 = new Point(point0.X, point0.Y + cachedFormattedChar.Height);

                    figure.Segments.Add(new LineSegment(point0, true));
                    figure.Segments.Add(new LineSegment(point2, true));
                    figure.Segments.Add(new LineSegment(point3, true));
                    figure.Segments.Add(new LineSegment(point1, true));
                    figure.Segments.Add(new LineSegment(point4, true));
                    figure.Segments.Add(new LineSegment(point5, true));
                    figure.Segments.Add(new LineSegment(point6, true));
                    figure.Segments.Add(new LineSegment(point7, true));

                    guidelines.GuidelinesX.Add(point6.X + halfPenThickness);
                    guidelines.GuidelinesX.Add(point0.X + halfPenThickness);
                    guidelines.GuidelinesX.Add(point1.X + halfPenThickness);
                    guidelines.GuidelinesX.Add(point2.X + halfPenThickness);
                    guidelines.GuidelinesY.Add(point0.Y + halfPenThickness);
                    guidelines.GuidelinesY.Add(point6.Y + halfPenThickness);
                    guidelines.GuidelinesY.Add(point1.Y + halfPenThickness);
                    guidelines.GuidelinesY.Add(point5.Y + halfPenThickness);
                }
                else
                {
                    // +---------------------------+
                    // |                           |
                    // |     0-------------1       |
                    // |     |             |       |
                    // |     3-------------2       |
                    // |                           |
                    // |                           |
                    // |                           |
                    // |                           |
                    // |                           |
                    // +---------------------------+
                    Point point2 = new Point(point1.X, point1.Y + cachedFormattedChar.Height);
                    Point point3 = new Point(point0.X, point0.Y + cachedFormattedChar.Height);

                    figure.Segments.Add(new LineSegment(point1, true));
                    figure.Segments.Add(new LineSegment(point2, true));
                    figure.Segments.Add(new LineSegment(point3, true));

                    guidelines.GuidelinesX.Add(point0.X + halfPenThickness);
                    guidelines.GuidelinesX.Add(point1.X + halfPenThickness);
                    guidelines.GuidelinesY.Add(point0.Y + halfPenThickness);
                    guidelines.GuidelinesY.Add(point3.Y + halfPenThickness);
                }
            }
            else
            {
                if ((long)(point0.Y + cachedFormattedChar.Height) == (long)point1.Y)
                {
                    // +---------------------------+
                    // |                           |
                    // |             0-------------2
                    // |             |             |
                    // 7--------1    4-------------3
                    // |        |                  |
                    // 6--------5                  |
                    // |                           |
                    // |                           |
                    // |                           |
                    // +---------------------------+
                    Point point2 = new Point(rhsVerticalLinePoint0.X - (CharsBetweenSections * cachedFormattedChar.Width) + selectionBoxXPadding, point0.Y);
                    Point point3 = new Point(rhsVerticalLinePoint0.X - (CharsBetweenSections * cachedFormattedChar.Width) + selectionBoxXPadding, point1.Y);
                    Point point4 = new Point(point0.X, point1.Y);

                    figure.Segments.Add(new LineSegment(point2, true));
                    figure.Segments.Add(new LineSegment(point3, true));
                    figure.Segments.Add(new LineSegment(point4, true));

                    guidelines.GuidelinesX.Add(point0.X + halfPenThickness);
                    guidelines.GuidelinesX.Add(point2.X + halfPenThickness);
                    guidelines.GuidelinesY.Add(point0.Y + halfPenThickness);
                    guidelines.GuidelinesY.Add(point4.Y + halfPenThickness);

                    PathFigure lhsFigure = new PathFigure
                    {
                        StartPoint = point1,
                        IsClosed = true,
                    };

                    Point point5 = new Point(point1.X, point1.Y + cachedFormattedChar.Height);
                    Point point6 = new Point(lhsVerticalLinePoint0.X + (CharsBetweenSections * cachedFormattedChar.Width) - selectionBoxXPadding, point1.Y + cachedFormattedChar.Height);
                    Point point7 = new Point(lhsVerticalLinePoint0.X + (CharsBetweenSections * cachedFormattedChar.Width) - selectionBoxXPadding, point1.Y);

                    lhsFigure.Segments.Add(new LineSegment(point5, true));
                    lhsFigure.Segments.Add(new LineSegment(point6, true));
                    lhsFigure.Segments.Add(new LineSegment(point7, true));

                    guidelines.GuidelinesX.Add(point7.X + halfPenThickness);
                    guidelines.GuidelinesX.Add(point1.X + halfPenThickness);
                    guidelines.GuidelinesY.Add(point7.Y + halfPenThickness);
                    guidelines.GuidelinesY.Add(point6.Y + halfPenThickness);

                    geometry.Figures.Add(lhsFigure);
                }
                else
                {
                    // +---------------------------+
                    // |                           |
                    // |             0-------------2
                    // |             |             |
                    // 6-------------7             |
                    // |                           |
                    // |        1------------------3
                    // |        |                  |
                    // 5--------4                  |
                    // |                           |
                    // +---------------------------+
                    Point point2 = new Point(rhsVerticalLinePoint0.X - (CharsBetweenSections * cachedFormattedChar.Width) + selectionBoxXPadding, point0.Y);
                    Point point3 = new Point(rhsVerticalLinePoint0.X - (CharsBetweenSections * cachedFormattedChar.Width) + selectionBoxXPadding, point1.Y);
                    Point point4 = new Point(point1.X, point1.Y + cachedFormattedChar.Height);
                    Point point5 = new Point(lhsVerticalLinePoint0.X + (CharsBetweenSections * cachedFormattedChar.Width) - selectionBoxXPadding, point1.Y + cachedFormattedChar.Height);
                    Point point6 = new Point(lhsVerticalLinePoint0.X + (CharsBetweenSections * cachedFormattedChar.Width) - selectionBoxXPadding, point0.Y + cachedFormattedChar.Height);
                    Point point7 = new Point(point0.X, point0.Y + cachedFormattedChar.Height);

                    figure.Segments.Add(new LineSegment(point0, true));
                    figure.Segments.Add(new LineSegment(point2, true));
                    figure.Segments.Add(new LineSegment(point3, true));
                    figure.Segments.Add(new LineSegment(point1, true));
                    figure.Segments.Add(new LineSegment(point4, true));
                    figure.Segments.Add(new LineSegment(point5, true));
                    figure.Segments.Add(new LineSegment(point6, true));
                    figure.Segments.Add(new LineSegment(point7, true));

                    guidelines.GuidelinesX.Add(point6.X + halfPenThickness);
                    guidelines.GuidelinesX.Add(point1.X + halfPenThickness);
                    guidelines.GuidelinesX.Add(point0.X + halfPenThickness);
                    guidelines.GuidelinesX.Add(point2.X + halfPenThickness);
                    guidelines.GuidelinesY.Add(point0.Y + halfPenThickness);
                    guidelines.GuidelinesY.Add(point6.Y + halfPenThickness);
                    guidelines.GuidelinesY.Add(point1.Y + halfPenThickness);
                    guidelines.GuidelinesY.Add(point5.Y + halfPenThickness);
                }
            }

            geometry.Figures.Add(figure);

            drawingContext.DrawGeometry(brush, pen, geometry);
            drawingContext.Pop();
        }

        private void UpdateState()
        {
            UpdateMaxVisibleRowsAndColumns();
            UpdateScrollBar();
        }

        private void UpdateMaxVisibleRowsAndColumns()
        {
            int maxVisibleRows = 0;
            int maxVisibleColumns = 0;

            if ((ShowAddress || ShowData || ShowText) && canvas != null)
            {
                // TODO: We should not be updating this every time. Cache it once if the font on the control changes. Same with typeface and use it throughout.
                cachedFormattedChar = new FormattedText("X", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, Foreground, 1.0);

                maxVisibleRows = Math.Max(0, (int)(canvas.ActualHeight / cachedFormattedChar.Height));

                if (ShowData || ShowText)
                {
                    int charsPerRow = (int)(canvas.ActualWidth / cachedFormattedChar.Width);

                    if (ShowAddress)
                    {
                        charsPerRow -= CalculateAddressColumnCharWidth() + (2 * CharsBetweenSections);
                    }

                    if (ShowData && ShowText)
                    {
                        charsPerRow -= 3 * CharsBetweenSections;
                    }

                    int charsPerColumn = 0;

                    if (ShowData)
                    {
                        charsPerColumn += CalculateDataColumnCharWidth() + CharsBetweenDataColumns;
                    }

                    if (ShowText)
                    {
                        charsPerColumn += CalculateTextColumnCharWidth();
                    }

                    if (charsPerColumn != 0)
                    {
                        maxVisibleColumns = Math.Max(0, charsPerRow / charsPerColumn);
                    }
                }
                else
                {
                    maxVisibleColumns = 0;
                }
            }

            MaxVisibleRows = maxVisibleRows;
            MaxVisibleColumns = maxVisibleColumns;

            // Maximum visible rows has now changed and so we must update the maximum amount we should scroll by
            verticalScrollBar.LargeChange = maxVisibleRows;
        }

        private void UpdateScrollBar()
        {
            if ((ShowAddress || ShowData || ShowText) && DataSource != null && Columns > 0 && MaxVisibleRows > 0)
            {
                long q = DataSource.BaseStream.Length / BytesPerRow;
                long r = DataSource.BaseStream.Length % BytesPerRow;

                // Each scroll value represents a single drawn row
                verticalScrollBar.Maximum = q + (r > 0 ? 1 : 0);

                // Adjust the scroll value based on the current offset
                verticalScrollBar.Value = Offset / BytesPerRow;

                // Adjust again to compensate for residual bytes if the number of bytes between the start of the stream
                // and the current offset is less than the number of bytes we can display per row
                if (verticalScrollBar.Value == 0 && Offset > 0)
                {
                    ++verticalScrollBar.Value;
                }
            }
            else
            {
                verticalScrollBar.Maximum = 0;
            }
        }

        private long ConvertPositionToOffset(Point position)
        {
            long offset = Offset;

            switch (highlightBegin)
            {
                case SelectionArea.Address:
                {
                    Point addressVerticalLinePoint0 = CalculateAddressVerticalLinePoint0();
                    Point addressVerticalLinePoint1 = CalculateAddressVerticalLinePoint1();

                    // Clamp the Y coordinate to within the address region
                    position.Y = position.Y.Clamp(addressVerticalLinePoint0.Y, addressVerticalLinePoint1.Y);

                    // Convert the Y coordinate to the row number
                    position.Y /= cachedFormattedChar.Height;

                    if (position.Y >= MaxVisibleRows)
                    {
                        // Due to floating point rounding we may end up with exactly the maximum number of rows, so adjust to compensate
                        --position.Y;
                    }

                    offset += BytesPerRow * (long)position.Y;
                }

                break;

                case SelectionArea.Data:
                {
                    Point addressVerticalLinePoint0 = CalculateAddressVerticalLinePoint0();

                    Point dataVerticalLinePoint0 = CalculateDataVerticalLinePoint0();
                    Point dataVerticalLinePoint1 = CalculateDataVerticalLinePoint1();

                    // Clamp the X coordinate to within the data region
                    position.X = position.X.Clamp(addressVerticalLinePoint0.X + (CharsBetweenSections * cachedFormattedChar.Width), dataVerticalLinePoint0.X - (CharsBetweenSections * cachedFormattedChar.Width));

                    // Normalize with respect to the data region
                    position.X -= addressVerticalLinePoint0.X + (CharsBetweenSections * cachedFormattedChar.Width);

                    // Convert the X coordinate to the column number
                    position.X /= (CalculateDataColumnCharWidth() + CharsBetweenDataColumns) * cachedFormattedChar.Width;

                    if (position.X >= Columns)
                    {
                        // Due to floating point rounding we may end up with exactly the maximum number of columns, so adjust to compensate
                        --position.X;
                    }

                    // Clamp the Y coordinate to within the data region
                    position.Y = position.Y.Clamp(dataVerticalLinePoint0.Y, dataVerticalLinePoint1.Y);

                    // Convert the Y coordinate to the row number
                    position.Y /= cachedFormattedChar.Height;

                    if (position.Y >= MaxVisibleRows)
                    {
                        // Due to floating point rounding we may end up with exactly the maximum number of rows, so adjust to compensate
                        --position.Y;
                    }

                    offset += (((long)position.Y * Columns) + (long)position.X) * BytesPerColumn;
                }

                break;

                case SelectionArea.Text:
                {
                    Point dataVerticalLinePoint0 = CalculateDataVerticalLinePoint0();

                    Point textVerticalLinePoint0 = CalculateTextVerticalLinePoint0();
                    Point textVerticalLinePoint1 = CalculateTextVerticalLinePoint1();

                    // Clamp the X coordinate to within the text region
                    position.X = position.X.Clamp(dataVerticalLinePoint0.X + (CharsBetweenSections * cachedFormattedChar.Width), textVerticalLinePoint0.X - (CharsBetweenSections * cachedFormattedChar.Width));

                    // Normalize with respect to the text region
                    position.X -= dataVerticalLinePoint0.X + (CharsBetweenSections * cachedFormattedChar.Width);

                    // Convert the X coordinate to the column number
                    position.X /= CalculateTextColumnCharWidth() * cachedFormattedChar.Width;

                    if (position.X >= Columns)
                    {
                        // Due to floating point rounding we may end up with exactly the maximum number of columns, so
                        // adjust to compensate
                        --position.X;
                    }

                    // Clamp the Y coordinate to within the text region
                    position.Y = position.Y.Clamp(textVerticalLinePoint0.Y, textVerticalLinePoint1.Y);

                    // Convert the Y coordinate to the row number
                    position.Y /= cachedFormattedChar.Height;

                    if (position.Y >= MaxVisibleRows)
                    {
                        // Due to floating point rounding we may end up with exactly the maximum number of rows, so adjust to compensate
                        --position.Y;
                    }

                    offset += (((long)position.Y * Columns) + (long)position.X) * BytesPerColumn;
                }

                break;

                default:
                {
                    throw new InvalidOperationException($"Invalid highlight state ${highlightState}");
                }
            }

            return offset;
        }

        private Point ConvertOffsetToPosition(long offset, SelectionArea relativeTo)
        {
            Point position = default;

            switch (relativeTo)
            {
                case SelectionArea.Data:
                {
                    Point addressVerticalLinePoint0 = CalculateAddressVerticalLinePoint0();

                    position.X = addressVerticalLinePoint0.X + (CharsBetweenSections * cachedFormattedChar.Width);
                    position.Y = addressVerticalLinePoint0.Y;

                    // Normalize requested offset to a zero based column
                    long normalizedColumn = (offset - Offset) / BytesPerColumn;

                    position.X += (((normalizedColumn % Columns) + Columns) % Columns) * (CalculateDataColumnCharWidth() + CharsBetweenDataColumns) * cachedFormattedChar.Width;

                    if (normalizedColumn < 0)
                    {
                        // Negative normalized offset means the Y position is above the current offset. Because division
                        // rounds toward zero we need to compensate here.
                        position.Y += (((normalizedColumn + 1) / Columns) - 1) * cachedFormattedChar.Height;
                    }
                    else
                    {
                        position.Y += normalizedColumn / Columns * cachedFormattedChar.Height;
                    }
                }

                break;

                case SelectionArea.Text:
                {
                    Point dataVerticalLinePoint0 = CalculateDataVerticalLinePoint0();

                    position.X = dataVerticalLinePoint0.X + (CharsBetweenSections * cachedFormattedChar.Width);
                    position.Y = dataVerticalLinePoint0.Y;

                    // Normalize requested offset to a zero based column
                    long normalizedColumn = (offset - Offset) / BytesPerColumn;

                    position.X += (((normalizedColumn % Columns) + Columns) % Columns) * CalculateTextColumnCharWidth() * cachedFormattedChar.Width;

                    if (normalizedColumn < 0)
                    {
                        // Negative normalized offset means the Y position is above the current offset. Because division
                        // rounds toward zero we need to compensate here.
                        position.Y += (((normalizedColumn + 1) / Columns) - 1) * cachedFormattedChar.Height;
                    }
                    else
                    {
                        position.Y += normalizedColumn / Columns * cachedFormattedChar.Height;
                    }
                }

                break;

                default:
                {
                    throw new ArgumentException($"Invalid relative area {relativeTo}", nameof(relativeTo));
                }
            }

            return position;
        }

        private bool IsOffsetVisible(long offset)
        {
            long maxBytesDisplayed = BytesPerRow * MaxVisibleRows;

            return Offset <= offset && Offset + maxBytesDisplayed >= offset;
        }

        private class CanvasVisualHost : UIElement
        {
            /// <summary>
            /// Gets or sets the Visual.
            /// </summary>
            public Visual Visual { get; set; }

            /// <inheritdoc/>
            protected override int VisualChildrenCount => Visual == null ? 0 : 1;

            /// <inheritdoc/>
            protected override Visual GetVisualChild(int index)
            {
                return Visual;
            }
        }
    }
}
