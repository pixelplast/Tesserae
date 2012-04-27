using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace Tesserae
{
    // Canvas tracks three coordinate systems:
    // 1. pWidth/Height: Canvas pixel count
    // 2. tWidth/Height: Canvas tile count
    public class Canvas
    {
        // User-defined (or derived) fields
        public int tMinWidth;
        public int tMinHeight;
        
        // Canvas size
        public int pWidth;          // Canvas pixel width
        public int pHeight;         // Canvas pixel height
        public float tileScale;     // Canvas-to-window scale
        
        // Testing: tHalo = 0 (should be at least 2)
        public int tWidth;      // # of (full) tiles per width
        public int tHeight;     // # of (full) tiles per height
        public int tHalo = 0;   // # of rendered tiles outside viewport
        
        // Camera position (focal tile)
        public int pX;      // Camera X position in pixels
        public int pY;      // Camera Y position in pixels
        public int tX;      // Tile.X containing camera
        public int tY;      // Tile.Y containing camera
        
        // Canvas loop hoisting
        public int tStartX;
        public int tEndX;
        public int tStartY;
        public int tEndY;
        
        // Mosaic parameters
        // (Note: Testing with default arguments)
        public int pTileWidth = 16;      // Tile width in pixels
        public int pTileHeight = 16;     // Tile height in pixels
        
        // Necessary?
        Game game;
        public Vector2 camera;
        
        public Canvas(Game gameInput)
        {
            game = gameInput;
            RescaleCanvas();
            game.Window.ClientSizeChanged += UpdateViewport;
        }
        
        public void RescaleCanvas()
        {
            // Screen pixel count
            var pWindowWidth = game.GraphicsDevice.Viewport.Bounds.Width;
            var pWindowHeight = game.GraphicsDevice.Viewport.Bounds.Height;            
            
            // Probably the best control parameters:
            // Define minimum tile width/height, allow to expand
            tMinWidth = 15;
            tMinHeight = 15;
            
            // Determine the minimum scaling
            var xScale = (float)pWindowWidth / (pTileWidth * tMinWidth);
            var yScale = (float)pWindowHeight / (pTileHeight * tMinHeight);
            tileScale = Math.Min(xScale, yScale);
            tWidth = (int)Math.Round(pWindowWidth / (pTileWidth * tileScale));
            tHeight = (int)Math.Round(pWindowHeight / (pTileHeight * tileScale));
            
            // Virtual height is prescribed (i.e. window-independent)
            pHeight = (int)Math.Round(pWindowHeight / tileScale);
            pWidth = (int)Math.Round(pWindowWidth / tileScale);
            
            // Initialize camera on centre or left/below centre pixel
            pX = (pWidth - 1) / 2;
            pY = (pHeight - 1) / 2;
            tX = pX / pTileWidth;
            tY = pY / pTileHeight;
            Console.WriteLine("tX, tWidth: {0}, {1}", tX, tWidth);
            
            camera = tileScale * (new Vector2((float)pX, (float)pY));
            
            // Loop hoisting
            tStartX = tX - tWidth / 2 + 1 - tHalo;
            tEndX = tX + tWidth / 2 + 1 + tHalo;
            tStartY = tY - tHeight / 2 + 1 - tHalo;
            tEndY = tY + tHeight / 2 + 1 + tHalo;
            
            Console.WriteLine("i: {0}..{1}", tStartX, tEndX);
            Console.WriteLine("j: {0}..{1}", tStartY, tEndY);
            
            // Testing
            Console.WriteLine(" Pixel Width: {0}", pWindowWidth);
            Console.WriteLine("Pixel Height: {0}", pWindowHeight);
            Console.WriteLine("      pX, pY: {0}, {1}", pX, pY);
        }
        
        public void UpdateViewport(object sender, EventArgs e)
        {
            game.Window.ClientSizeChanged -= UpdateViewport;
            RescaleCanvas();
            game.Window.ClientSizeChanged += UpdateViewport;
        }
    }
}
