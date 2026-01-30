using UnityEngine;

public static class SpriteHelper
{
    private static Sprite _whiteSquare;
    
    /// <summary>
    /// Get or create a simple white square sprite
    /// </summary>
    public static Sprite GetWhiteSquare()
    {
        if (_whiteSquare != null) return _whiteSquare;
        
        // Create a 1x1 white texture
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        
        // Create sprite from texture
        _whiteSquare = Sprite.Create(
            texture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f
        );
        
        return _whiteSquare;
    }
}