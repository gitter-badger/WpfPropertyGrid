namespace WpfPropertyGrid
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class SettingControl : Control
    {
        public static readonly object Default = new object();
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(string),
            typeof(SettingControl),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(object),
            typeof(SettingControl),
            new FrameworkPropertyMetadata
            {
                PropertyChangedCallback = OnValueChanged,
                DefaultValue = Default,
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });

        public static readonly DependencyProperty OldValueProperty = DependencyProperty.Register(
            "OldValue",
            typeof(object),
            typeof(SettingControl),
            new FrameworkPropertyMetadata
            {
                PropertyChangedCallback = OnOldValueChanged,
                DefaultValue = Default,
                BindsTwoWayByDefault = false,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });

        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register(
            "IsDirty",
            typeof(bool),
            typeof(SettingControl),
            new PropertyMetadata(default(bool)));

        static SettingControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingControl), new FrameworkPropertyMetadata(typeof(SettingControl)));
        }

        public string Header
        {
            get
            {
                return (string)GetValue(HeaderProperty);
            }
            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        public object Value
        {
            get
            {
                return GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public object OldValue
        {
            get
            {
                return (object)GetValue(OldValueProperty);
            }
            set
            {
                SetValue(OldValueProperty, value);
            }
        }

        public bool IsDirty
        {
            get
            {
                return (bool)GetValue(IsDirtyProperty);
            }
            set
            {
                SetValue(IsDirtyProperty, value);
            }
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var c = (SettingControl)o;
            if (c.OldValue == Default)
            {
                var oldValueBinding = BindingOperations.GetBinding(c, OldValueProperty);
                if (oldValueBinding == null)
                {
                    var valueBinding = BindingOperations.GetBinding(c, ValueProperty);
                    if (valueBinding != null)
                    {
                        var binding = new Binding
                        {
                            Mode = BindingMode.OneWay,
                            Source = c.DataContext,
                            Path = c.GetOldValuePath(valueBinding.Path),
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        };
                        BindingOperations.SetBinding(c, OldValueProperty, binding);
                    }
                }
            }

            c.SetValue(IsDirtyProperty, !Equals(c.OldValue, c.Value));
        }

        private static void OnOldValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var c = (SettingControl)o;
            c.SetValue(IsDirtyProperty, !Equals(c.OldValue, c.Value));
        }

        private PropertyPath GetOldValuePath(PropertyPath settingsPath)
        {
            const string setting = "Setting";
            const string cache = "CachedSetting";
            if (!settingsPath.Path.StartsWith(setting))
            {
                throw new ArgumentException("Getting cached value like this is probably not good enough, it saves a lot of code though. Throwing this exception instead of silently failing");
            }
            string newPath = settingsPath.Path
                                         .Replace(setting, cache);
            return new PropertyPath(newPath);
        }
    }
}
