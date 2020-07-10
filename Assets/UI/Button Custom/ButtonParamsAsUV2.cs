using System;
namespace UnityEngine.UI
{
  [AddComponentMenu("UI/Effects/ButtonParamsAsUV2 As UV2", 16)]
  public class ButtonParamsAsUV2 : BaseMeshEffect
  {
    protected ButtonParamsAsUV2() { }
    public bool grayscale, selected;
    public override void ModifyMesh(VertexHelper vh) {
      UIVertex vert = new UIVertex();
      for(int i = 0; i < vh.currentVertCount; i++)
      {
        vh.PopulateUIVertex(ref vert, i);
        vert.uv2 = new Vector2(Convert.ToSingle(grayscale), Convert.ToSingle(selected));
        vh.SetUIVertex(vert, i);
      }
    }
  }
}
