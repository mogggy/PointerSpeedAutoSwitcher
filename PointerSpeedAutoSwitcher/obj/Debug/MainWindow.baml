   M S B A M L   `   `   `  ����S  OPointerSpeedAutoSwitcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:PointerSpeedAutoSwitcherPointerSpeedAutoSwitcher  5   	   )    #PointerSpeedAutoSwitcher.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   8local&clr-namespace:PointerSpeedAutoSwitcher     5   	   -   5          �Name$  
mainWindow��5   	    � Title$! Pointer-Speed Auto-Switcher��5   	   $	��350��6-   $	��525��6:    � Icon$ images/cheese.ico��5	   	      CustomWindowStyle#��[  5
   	   .��5      � $��2q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��3* �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   �� $��1��6   $��1��6    $��Process Name��6-   $��Center=�6D   
�������6a   �� 5   
   $��2��6   $��1��6    $��Speed (1-20)��6-   $��Center=�6D   
�������6a   �� 5   
   $��1��6   $��2��6"   $��2q�6/   $��Default��6:    �� TextAlignment$ Center=�6I   $��2q�6`    �� 
IsReadOnly .   StandardTextBoxStyle#��[ 6~   �� 5    
   -   5         ��Name$ tbDefaultSense��5       $��2��6*   $��2��6:   $��2q�6G   $ Center=�6R   $��10��6i   #��[ 6s   �� 5!   
   -   5         ��Name$ btGetCurrent��5!      $��3��6'   $��2��67   $��Get Current��6D   $��2q�6Z     StandardButtonStyle#��[ 6�   �� 5"   
   -   5        $ btSetCurrent��5"      $��4��6'   $��2��67   $��Set Current��6D   $��2q�6Z   #��[ 6�   �� 5$   
   -   5        $ tbProcessName��5$      $��1��6)   $��3��69   $��2q�6F   $ Center=�6Q   $��2q�6h   $��moggySleep.exe��6t   #��[ 6�   �� 5%   
   -   5        $ tbProcessSense��5%      $��2��6*   $��3��6:   $��2q�6G   $ Center=�6R   $��14��6i   #��[ 6s   �� 5'   
   -   5        $ tbLog��5'      $��1��6!   $��2��61   $��4��6E   $��2��6R   $�� ��6c   $��2q�6k   $��2q�6v    . #��[ 6�   �� 5(   
   -   5        $ btStart��5(      $��3��6"   $��4��62   $��2q�6?   {�.  $��Start��6r   $
��Auto��6�   #��[ 6�   �� 5)   
   -	   5        $ btEnd��5)      $��4��6    $��4��60   $��2q�6=   $
��Stop��6\   $
��Auto��6k   #��[ 6y   5*      5+      