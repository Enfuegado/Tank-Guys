using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ArenaView : MonoBehaviour
{
    [SerializeField] private float radius = 8f;
    [SerializeField] private Color color = Color.yellow;

    private void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();

        sr.sprite = GenerateCircleSprite();
        sr.color = color;

        transform.localScale = Vector3.one * radius * 2f;
    }

    private Sprite GenerateCircleSprite()
    {
        int size = 256;
        Texture2D texture = new Texture2D(size, size);

        Color clear = new Color(0, 0, 0, 0);

        Vector2 center = new Vector2(size / 2f, size / 2f);
        float r = size / 2f;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);

                if (dist <= r)
                    texture.SetPixel(x, y, Color.white);
                else
                    texture.SetPixel(x, y, clear);
            }
        }

        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            size
        );
    }
}