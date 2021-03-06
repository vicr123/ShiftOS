﻿/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using Newtonsoft.Json;
using ShiftOS.Engine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShiftOS.WinForms.Applications
{
    [Launcher("ShiftLetters", false, null, "Games")]
    [RequiresUpgrade("shiftletters")]
    [WinOpen("shiftletters")]
    [DefaultIcon("iconShiftLetters")]
    public partial class ShiftLetters : UserControl, IShiftOSWindow
    {
        int lives = 7;
        string word = "";
        static Random rng = new Random();
        string guessedCharacters = "";

        public ShiftLetters()
        {
            InitializeComponent();
        }
        
        private void StartGame()
        {
            guessedCharacters = "";
            lives = 7;
            tbguess.Visible = true;
            lbllives.Visible = true;
            lblword.Visible = true;
            btnrestart.Visible = false;
            var wordlist = new List<string>
            {
                "shiftos",
                "devx",
                "artpad",
                "shifter",
                "pong",
                "shiftorium",
                "codepoints",
                "shiftletters",
                "shops",
                "mud",
                "notification",
                "namechanger",
                "skinning",
                "skinloader",
                "calculator",
                "fileskimmer",
                "lua",
                "shiftnet",
                "terminal",
                "textpad"
            };
            //This can diversify the amount of ShiftOS-related words in the game.
            foreach(var upg in Shiftorium.GetDefaults())
            {
                foreach(var w in upg.Name.Split(' '))
                {
                    if (!wordlist.Contains(w.ToLower()))
                        wordlist.Add(w.ToLower());
                }
            }
            word = wordlist[rng.Next(wordlist.Count)];
            while(word == lastword)
            {
                word = wordlist[rng.Next(wordlist.Count)];
            }
            lastword = word; //to make the game not choose the same word twice or more in a row
            lbllives.Text = "You have 7 lives left!";
            lblword.Text = "";
            for (int i=0; i<word.Length; i++)
            {
                lblword.Text = lblword.Text + "_ ";
            }
        }

        public void OnLoad()
        {
            StartGame();
        }

        public void OnUpgrade()
        {

        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnSkinLoad()
        {

        }

        string lastword = "";

        private void tbguess_TextChanged(object sender, EventArgs e)
        {
            if (this.tbguess.Text.Length == 1)
            {
                var charGuessed = this.tbguess.Text.ToLower();
                bool correct = false;
                this.tbguess.Text = "";
                for (int i=0; i < word.Length; i++)
                {
                    if (word[i] == System.Convert.ToChar(charGuessed) & lives > 0)
                    {
                        char[] letters = lblword.Text.ToCharArray();
                        correct = true;
                        letters[i * 2] = System.Convert.ToChar(charGuessed);
                        lblword.Text = string.Join("", letters);
                        if (!lblword.Text.Contains("_"))
                        {
                            int oldlives = lives;
                            tbguess.Visible = false;
                            lives = 0;
                            lbllives.Visible = true;
                            btnrestart.Visible = true;
                            int cp = word.Length * oldlives;
                            lbllives.Text = "You earned: " + cp + " codepoints!";
                            SaveSystem.TransferCodepointsFrom("shiftletters", cp);
                        }
                    }
                }
                if (correct == false & lives > 0 & !guessedCharacters.Contains(charGuessed))
                {
                    guessedCharacters = guessedCharacters + charGuessed;
                    lives--;
                    lbllives.Text = "You have: " + lives + " lives left!";
                    if (lives == 0)
                    {
                        tbguess.Visible = false;
                        lbllives.Visible = false;
                        btnrestart.Visible = true;
                    }
                }
            }
        }

        private void btnrestart_Click(object sender, EventArgs e)
        {
            StartGame();
        }
    }
}
