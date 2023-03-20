using UnityEngine;

public static class SkinnedMeshExtension
{
    public static void CopyBonesFrom (this SkinnedMeshRenderer target, SkinnedMeshRenderer source)
    {
        target.bones = source.bones;
        target.rootBone = source.rootBone;
    }
}
