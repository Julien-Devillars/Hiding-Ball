using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall
{
    private List<GameObject> mWalls;
    private List<List<Vector3>> mShadowQuads;
    private Color mColor;

    public Wall()
    {
        mWalls = new List<GameObject>();
        mShadowQuads = new List<List<Vector3>>();
        mColor = Color.white;
    }
    public void addWall(GameObject gameobject)
    {
        mWalls.Add(gameobject);
    }

    public void setWalls(List<GameObject> _walls)
    {
        mWalls = _walls;
    }

    public List<GameObject> getWalls()
    {
        return mWalls;
    }

    public void setColor(Color _color)
    {
        mColor = _color;
    }

    public Color getColor()
    {
        return mColor;
    }

    public void addShadowQuad(List<Vector3> _shadow_quad)
    {
        mShadowQuads.Add(_shadow_quad);
    }

    public void setShadowQuads(List<List<Vector3>> _shadow_quads)
    {
        mShadowQuads = _shadow_quads;
    }
    public List<List<Vector3>> getShadowQuads()
    {
        return mShadowQuads;
    }

    public void clearShadowQuad()
    {
        mShadowQuads.Clear();
    }

    public void drawShadowQuad()
    {
        for (int i = 0; i < mShadowQuads.Count; i++)
            for (int j = 0; j < mShadowQuads[i].Count; ++j)
                for (int k = 0; k < 5; ++k) // Used only to see better the lines
                    Debug.DrawLine(mShadowQuads[i][j], mShadowQuads[i][(j + 1) % mShadowQuads[i].Count], mColor, 4f);
    }
}
