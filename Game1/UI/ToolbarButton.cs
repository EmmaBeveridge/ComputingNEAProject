﻿
using Game1.GOAP;
using Game1.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.UI
{
    
    
    
    
    public abstract class ToolbarButton : Button
    {

        public ToolbarPanel panel;
        public static Vector2 toolbarButtonTextureDim = new Vector2(50);

        public ToolbarButton(string argLabel, Vector2 argPosition, Texture2D argTexture) : base(argLabel, argPosition, argTexture)
        {

            

        }

    }

    public class NeedsButton : ToolbarButton
    {

        public static Texture2D defaultTexture;

        public NeedsButton(string argLabel, Vector2 argPosition, List<NeedBar> needBars) : base(argLabel, argPosition, defaultTexture)
        {
            panel = new NeedsPanel(needBars);
        }

    }

    public class RelationshipsButton : ToolbarButton
    {

        public static Texture2D defaultTexture;

        public RelationshipsButton(string argLabel, Vector2 argPosition, Dictionary<People, int> argRelationships) : base(argLabel, argPosition, defaultTexture)
        {
            panel = new RelationshipsPanel(argRelationships);
        }

    }

    public class SkillsButton : ToolbarButton
    {

        public static Texture2D defaultTexture;

        public SkillsButton(string argLabel, Vector2 argPosition) : base(argLabel, argPosition, defaultTexture)
        {
            panel = new SkillsPanel();
        }

    }
    public class CareerButton : ToolbarButton
    {

        public static Texture2D defaultTexture;

        public CareerButton(string argLabel, Vector2 argPosition) : base(argLabel, argPosition, defaultTexture)
        {
            panel = new CareerPanel();
        }

    }









}
