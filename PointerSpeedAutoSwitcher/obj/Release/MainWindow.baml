   M S B A M L   `   `   `  ���� S  OPointerSpeedAutoSwitcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:PointerSpeedAutoSwitcherPointerSpeedAutoSwitcher  )    #PointerSpeedAutoSwitcher.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     8local&clr-namespace:PointerSpeedAutoSwitcher     -     �Name$  
mainWindow�� � Title$! Pointer-Speed Auto-Switcher��$	��350��$	��525�� � Icon$ images/cheese.ico��   CustomWindowStyle#��[  .��� $��2q� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $��* � � ColumnDefinitions �� $
��Auto ��� $��3* ��� $��* ��� $��* ��� $��* �.+��� $��1��$��1��$��Process Name��$��Center=�
��������� $��2��$��1��$��Speed (1-20)��$��Center=�
��������� $��1��$��2��$��2q�$��Default�� �� TextAlignment$ Center=�$��2q� �� 
IsReadOnly .   StandardTextBoxStyle#��[ �� -    ��Name$ tbDefaultSense��$��2��$��2��$��2q�$ Center=�$��10��#��[ �� -    ��Name$ btGetCurrent��$��3��$��2��$��Get Current��$��2q�  StandardButtonStyle#��[ �� -   $ btSetCurrent��$��4��$��2��$��Set Current��$��2q�#��[ �� -   $ tbProcessName��$��1��$��3��$��2q�$ Center=�$��2q�$��moggySleep.exe��#��[ �� -   $ tbProcessSense��$��2��$��3��$��2q�$ Center=�$��14��#��[ �� -   $ tbLog��$��1��$��2��$��4��$��2��$�� ��$��2q�$��2q� . #��[ �� -   $ btStart��$��3��$��4��$��2q�{�.  $��Start��$
��Auto��#��[ �� -	   $ btEnd��$��4��$��4��$��2q�$
��Stop��$
��Auto��#��[ 