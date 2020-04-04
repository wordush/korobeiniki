using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Pohuj : MonoBehaviour
{

    [FormerlySerializedAs("R")] public float r; // Коэффициент скалистости
    [FormerlySerializedAs("GRAIN")] public int grain = 8; // Коэффициент зернистости
    [FormerlySerializedAs("FLAT")] public bool flat = false; // Делать ли равнины
    public Material material;



    private int _width = 2048;
    private int _height = 2048;
    private float _wh;
    private Color32[] _cols;
    private Texture2D _texture;


    void Start()
    {
        int resolution = _width;
        _wh = (float)_width + _height;

        // Задаём карту высот
        Terrain terrain = FindObjectOfType<Terrain>();
        float[,] heights = new float[resolution, resolution];

        // Создаём карту высот
        _texture = new Texture2D(_width, _height);
        _cols = new Color32[_width * _height];
        DrawPlasma(_width, _height);
        _texture.SetPixels32(_cols);
        _texture.Apply();

        // Используем шейдер (смотри пункт 3 во 2 части)
        material.SetTexture("_HeightTex", _texture);

        // Задаём высоту вершинам по карте высот
        for (int i = 0; i < resolution; i++)
        {
            for (int k = 0; k < resolution; k++)
            {
                heights[i, k] = _texture.GetPixel(i, k).grayscale * r;
            }
        }

        // Применяем изменения
        terrain.terrainData.size = new Vector3(_width, _width, _height);
        terrain.terrainData.heightmapResolution = resolution;
        terrain.terrainData.SetHeights(0, 0, heights);

        
    }

    // Считаем рандомный коэффициент смещения для высоты
    float Displace(float num)
    {
        float max = num / _wh * grain;
        return Random.Range(-0.5f, 0.5f) * max;
    }

    // Вызов функции отрисовки с параметрами
    void DrawPlasma(float w, float h)
    {
        float c1, c2, c3, c4;

        c1 = Random.value;
        c2 = Random.value;
        c3 = Random.value;
        c4 = Random.value;

        Divide(0.0f, 0.0f, w, h, c1, c2, c3, c4);
    }

    // Сама рекурсивная функция отрисовки
    void Divide(float x, float y, float w, float h, float c1, float c2, float c3, float c4)
    {

        float newWidth = w * 0.5f;
        float newHeight = h * 0.5f;

        if (w < 1.0f && h < 1.0f)
		{
            float c = (c1 + c2 + c3 + c4) * 0.25f;
            _cols[(int)x + (int)y * _width] = new Color(c, c, c);
        }
		else
		{
            float middle = (c1 + c2 + c3 + c4) * 0.25f + Displace(newWidth + newHeight);
            float edge1 = (c1 + c2) * 0.5f;
            float edge2 = (c2 + c3) * 0.5f;
            float edge3 = (c3 + c4) * 0.5f;
            float edge4 = (c4 + c1) * 0.5f;

            if (!flat)
            {
                if (middle <= 0)
                {
                    middle = 0;
                }
                else if (middle > 1.0f)
                {
                    middle = 1.0f;
                }
            }
            Divide(x, y, newWidth, newHeight, c1, edge1, middle, edge4);
            Divide(x + newWidth, y, newWidth, newHeight, edge1, c2, edge2, middle);
            Divide(x + newWidth, y + newHeight, newWidth, newHeight, middle, edge2, c3, edge3);
            Divide(x, y + newHeight, newWidth, newHeight, edge4, middle, edge3, c4);
        }
    }
}
