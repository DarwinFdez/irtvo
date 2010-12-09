﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
// additional
using System.IO;
using Ini;

namespace iRTVO
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public Options()
        {
            InitializeComponent();
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            apply();
        }


        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            apply();
            this.Close();
        }

        private void apply()
        {
            ComboBoxItem cbi = (ComboBoxItem)comboBoxTheme.SelectedItem;
            Properties.Settings.Default.theme = cbi.Content.ToString();

            int selected = 0;
            int i = 0;

            cbi = (ComboBoxItem)comboBoxLanguage.SelectedItem;
            foreach (String lang in Enum.GetNames(typeof(localization.Language)))
            {
                if (lang == (string)cbi.Content)
                    selected = i;
                i++;
            }

            Properties.Settings.Default.language = selected;
            Properties.Settings.Default.Save();

            saveOverlaySize();
            saveOverlayPos();

            SharedData.requestRefresh = true;

            try
            {
                if (Int32.Parse(textBoxUpdateFreq.Text) > 0)
                    Properties.Settings.Default.UpdateFrequency = Int32.Parse(textBoxUpdateFreq.Text);
                else
                    MessageBox.Show("Update frequency needs to be larger than zero");
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Update frequency needs to be larger than zero");
            }
        }

        private void saveOverlaySize()
        {
            int w = -1;
            int h = -1;
            try
            {
                w = Int32.Parse(textBoxSizeW.Text);
                h = Int32.Parse(textBoxSizeH.Text);
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Overlay size needs to be larger than one");
            }

            if (w >= 0 && h >= 0)
            {

                Properties.Settings.Default.OverlayWidth = w;
                Properties.Settings.Default.OverlayHeight = h;
                Properties.Settings.Default.Save();
            }
        }

        private void saveOverlayPos()
        {
            int w = -1;
            int h = -1;
            try
            {
                w = Int32.Parse(textBoxPosX.Text);
                h = Int32.Parse(textBoxPosY.Text);
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Overlay position needs to be larger than one");
            }

            if (w >= 0 && h >= 0)
            {

                Properties.Settings.Default.OverlayLocationX = w;
                Properties.Settings.Default.OverlayLocationY = h;
                Properties.Settings.Default.Save();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxPosX.Text = Properties.Settings.Default.OverlayLocationX.ToString();
            textBoxPosY.Text = Properties.Settings.Default.OverlayLocationY.ToString();
            textBoxSizeW.Text = Properties.Settings.Default.OverlayWidth.ToString();
            textBoxSizeH.Text = Properties.Settings.Default.OverlayHeight.ToString();

            // get available themes
            IniFile settings;
            ComboBoxItem cboxitem;

            DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\themes\\");
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis) {
                if (File.Exists(Directory.GetCurrentDirectory() + "\\themes\\" + di.Name + "\\settings.ini"))
                {
                    settings = new IniFile(Directory.GetCurrentDirectory() + "\\themes\\" + di.Name + "\\settings.ini");
                    cboxitem = new ComboBoxItem();
                    cboxitem.Content = settings.IniReadValue("General", "name");
                    comboBoxTheme.Items.Add(cboxitem);
                }
            }

            comboBoxTheme.Text = Properties.Settings.Default.theme;
            settings = new IniFile(Directory.GetCurrentDirectory() + "\\themes\\" + Properties.Settings.Default.theme + "\\settings.ini");
            labelThemeAuthor.Content = "Author: " + settings.IniReadValue("General", "author");
            labelThemeSize.Content = "Original size: " + settings.IniReadValue("General", "width") + "x" + settings.IniReadValue("General", "height");

            // set language
            foreach (String lang in Enum.GetNames(typeof(localization.Language)))
            {
                cboxitem = new ComboBoxItem();
                cboxitem.Content = lang;
                comboBoxLanguage.Items.Add(cboxitem);
            }
            comboBoxLanguage.Text = ((localization.Language)Properties.Settings.Default.language).ToString();

            textBoxUpdateFreq.Text = Properties.Settings.Default.UpdateFrequency.ToString();
        }

        private void comboBoxTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IniFile settings;

            ComboBoxItem cbi = (ComboBoxItem)comboBoxTheme.SelectedItem;
            settings = new IniFile(Directory.GetCurrentDirectory() + "\\themes\\" + cbi.Content.ToString() + "\\settings.ini");
            labelThemeAuthor.Content = "Author: " + settings.IniReadValue("General", "author");
            labelThemeSize.Content = "Original size: " + settings.IniReadValue("General", "width") + "x" + settings.IniReadValue("General", "height");
        }
    }
}
