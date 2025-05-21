# Manual

## How to start?

- Click Scenes/DemoScene and press play button

## Files

- Animations ... Demo stand/walk animations
- Documents ... Manual
- Materials
    - Original base materials
    - Man*/ ... Materials for individual model parts
    - Woman*/ ... Materials for individual model parts
- Models ... FBX files
    - MP_Male_A1F ... Base model
    - MP_Female_A1F ... Base model
    - MP_Hair_* ... Hair under head_end
    - MP_Eyes_* ... Eyes under head_end
    - MP_Mouth_* ... Mouth under head_end
    - Glasses*,Sunglasses* ... Glasses under head_end
- Prefabs ... Prefab data
    - MP_Male_A1 Man* ... Individual models
    - MP_Female_A1 Woman* ... Individual models
    - Glasses ... Glasses parts
    - Hair ... Hair parts
- Scenes ... Demo scene
- Textures ... Texture .png files for material
- README.md ... Information

## Model structure

- Name: MP_[Type]_[ModelNo] [Name]
    - metarig_[type] ... Rigged humanoid bones
        - hips/spine/chest/upper_chest/neck/head/head_end
            - Hair ... Mesh, Material
            - Eyes ... Mesh, Material
            - Mouth ... Mesh, Material
            - Glasses ... Mesh, Material
        - others
    - Head ... Skinned Mesh, Material (part of HeadBody), Blend Shapes
    - Body ... Skinned Mesh, Material (part of HeadBody)
    - Tops ... Skinned Mesh, Material
    - Bottoms ... Skinned Mesh, Material
    - Shoes ... Skinned Mesh, Material

## Unity Humanoid Avatar

- The model is compatible with Unity's Humanoid Avatar system and includes full body, head (with neck), 
and hand articulation down to the individual fingers.

## How to create a new custom model?

- Drag one Prefab onto the Scene.
- Select the Body/Tops/Bottoms/Shoes game object under the character game object, 
and replace the relevant Mesh item from the mesh list at the Skinned Mesh Renderer.
- Select the Hair/Eyes/Mouth/Glasses game object under the head_end game object, 
and replace the relevant Mesh item from the mesh list at the Mesh Renderer.
- Select the Hair/Eye/Mouth/Glasses/Head/Body/Tops/Bottoms/Shoes Materials under the character game object, 
and replace the (new created or shared) materials' texture references.

## Face Customization

- BlendShapes
    - Select the "MP_Head_[Type]" game object and open its Skinned Mesh Renderer component.
    - Open the BlendShapes section.
    - Edit the values within the range of -100 to 100 (negative values can be entered).
        - NoseHeight
        - NoseSize
        - EyeAreaDepth
        - JawWidth
        - ChinLength
        - EarShape
        - EarSize
- Eyes and Mouth
    - Select the "MP_Eyes_[Type]" and "MP_Mouth_[Type]" game objects.
    - Modify their Position and Scale values in the Transform component.

## Glasses color customization

- Each "Glasses" item uses a single texture.
- To adjust the color of the glasses, modify the "Offset X" and "Offset Y" values within the "Surface Inputs" section of the "Glasses" material.