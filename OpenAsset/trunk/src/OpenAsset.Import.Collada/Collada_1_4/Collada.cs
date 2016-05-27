using System.Xml.Serialization;
using System.Collections.Generic;
using System;

[System.Xml.Serialization.XmlTypeAttribute( Namespace = "http://www.collada.org/2005/11/COLLADASchema" )]
[System.Xml.Serialization.XmlRootAttribute( "COLLADA", Namespace = "http://www.collada.org/2005/11/COLLADASchema", IsNullable = false )]
public class Collada : ColladaBase
{


	private string versionField;
	[XmlAttribute( "version")]
	public string version
	{
		get
		{
			return this.versionField;
		}
		set
		{
			if ( value == null )
				throw new ArgumentNullException( "version", "version attribute is required." );
			this.versionField = value;
			this.RaisePropertyChanged( "version" );
		}
	}

	private Asset assetField;
	[XmlElement( "asset")]
	public Asset asset
	{
		get
		{
			return this.assetField;
		}
		set
		{
			this.assetField = value;
			this.RaisePropertyChanged( "asset" );
		}
	}

	//private List<LibraryAnimations> libraryAnimationsField;

	//private List<LibraryAnimationClips> libraryAnimationClipsField;

	//private List<LibraryCameras> libraryCamerasField;

	//private List<LibraryControllers> libraryControllersField;

	//private List<LibraryGeometries> libraryGeometriesField;

	//private List<LibraryEffects> libraryEffectsField;

	//private List<LibraryForceFields> libraryForceFieldsField;

	//private List<LibraryImages> libraryImagesField;

	//private List<LibraryLights> libraryLightsField;

	//private List<LibraryMaterials> libraryMaterialsField;

	//private List<LibraryNodes> libraryNodesField;

	//private List<LibraryPhysicsMaterials> libraryPhysicsMaterialsField;

	//private List<LibraryPhysicsModels> libraryPhysicsModelsField;

	//private List<LibraryPhysicsScenes> libraryPhysicsScenesField;

	//private List<LibraryVisualScenes> libraryVisualScenesField;

	//private Scene sceneField;

	//private List<Extra> extraField;

	//[XmlElement("library_animations")]
	//public List<LibraryAnimations> libraryanimations
	//{
	//    get {
	//        return this.libraryAnimationsField;
	//    }
	//    set {
	//        this.libraryAnimationsField = value;
	//        this.RaisePropertyChanged("libraryanimations");
	//    }
	//}

	//[XmlElement("library_animation_clips")]
	//public List<LibraryAnimationClips> libraryanimationclips
	//{
	//    get
	//    {
	//        return this.libraryAnimationClipsField;
	//    }
	//    set
	//    {
	//        this.libraryAnimationClipsField = value;
	//        this.RaisePropertyChanged("libraryanimationclips");
	//    }
	//}

	//[XmlElement("library_cameras")]
	//public List<LibraryCameras> librarycameras
	//{
	//    get
	//    {
	//        return this.libraryCamerasField;
	//    }
	//    set
	//    {
	//        this.libraryCamerasField = value;
	//        this.RaisePropertyChanged("librarycameras");
	//    }
	//}

	//[XmlElement("library_controllers")]
	//public List<LibraryControllers> librarycontrollers
	//{
	//    get
	//    {
	//        return this.libraryControllersField;
	//    }
	//    set
	//    {
	//        this.libraryControllersField = value;
	//        this.RaisePropertyChanged("librarycontrollers");
	//    }
	//}

	//[XmlElement("library_geometries")]
	//public List<LibraryGeometries> librarygeometries
	//{
	//    get
	//    {
	//        return this.libraryGeometriesField;
	//    }
	//    set
	//    {
	//        this.libraryGeometriesField = value;
	//        this.RaisePropertyChanged("librarygeometries");
	//    }
	//}

	//[XmlElement("library_effects")]
	//public List<LibraryEffects> libraryeffects
	//{
	//    get
	//    {
	//        return this.libraryEffectsField;
	//    }
	//    set
	//    {
	//        this.libraryEffectsField = value;
	//        this.RaisePropertyChanged("libraryeffects");
	//    }
	//}

	//[XmlElement("library_force_fields")]
	//public List<LibraryForceFields> libraryforcefields
	//{
	//    get
	//    {
	//        return this.libraryForceFieldsField;
	//    }
	//    set
	//    {
	//        this.libraryForceFieldsField = value;
	//        this.RaisePropertyChanged("libraryforcefields");
	//    }
	//}

	//[XmlElement("library_images")]
	//public List<LibraryImages> libraryImages
	//{
	//    get
	//    {
	//        return this.libraryImagesField;
	//    }
	//    set
	//    {
	//        this.libraryImagesField = value;
	//        this.RaisePropertyChanged("libraryimagesfield");
	//    }
	//}

	//[XmlElement("library_lights")]
	//public List<LibraryLights> librarylights
	//{
	//    get
	//    {
	//        return this.libraryLightsField;
	//    }
	//    set
	//    {
	//        this.libraryLightsField = value;
	//        this.RaisePropertyChanged("librarylights");
	//    }
	//}

	//[XmlElement("library_materials")]
	//public List<LibraryMaterials> librarymaterials
	//{
	//    get
	//    {
	//        return this.libraryMaterialsField;
	//    }
	//    set
	//    {
	//        this.libraryMaterialsField = value;
	//        this.RaisePropertyChanged("librarymaterials");
	//    }
	//}

	//[XmlElement("library_nodes")]
	//public List<LibraryNodes> librarynodes
	//{
	//    get
	//    {
	//        return this.libraryNodesField;
	//    }
	//    set
	//    {
	//        this.libraryNodesField = value;
	//        this.RaisePropertyChanged("librarynodes");
	//    }
	//}

	//[XmlElement("library_physics_materials")]
	//public List<LibraryPhysicsMaterials> libraryphysicsmaterials
	//{
	//    get
	//    {
	//        return this.libraryPhysicsMaterialsField;
	//    }
	//    set
	//    {
	//        this.libraryPhysicsMaterialsField = value;
	//        this.RaisePropertyChanged("libraryphysicsmaterials");
	//    }
	//}

	//[XmlElement("library_physics_models")]
	//public List<LibraryPhysicsModels> libraryphysicsmodels
	//{
	//    get
	//    {
	//        return this.libraryPhysicsModelsField;
	//    }
	//    set
	//    {
	//        this.libraryPhysicsModelsField = value;
	//        this.RaisePropertyChanged("libraryphysicsmodels");
	//    }
	//}

	//[XmlElement("library_physics_scenes")]
	//public List<LibraryPhysicsScenes> libraryphysicsscenes
	//{
	//    get
	//    {
	//        return this.libraryPhysicsScenesField;
	//    }
	//    set
	//    {
	//        this.libraryPhysicsScenesField = value;
	//        this.RaisePropertyChanged("libraryphysicsscenes");
	//    }
	//}

	//[XmlElement("library_visual_scenes")]
	//public List<LibraryVisualScenes> libraryvisualscenes
	//{
	//    get
	//    {
	//        return this.libraryVisualScenesField;
	//    }
	//    set
	//    {
	//        this.libraryVisualScenesField = value;
	//        this.RaisePropertyChanged("libraryvisualscenes");
	//    }
	//}

	//[XmlElement("scene")]
	//public Scene scene
	//{
	//    get
	//    {
	//        return this.sceneField;
	//    }
	//    set
	//    {
	//        this.sceneField = value;
	//        this.RaisePropertyChanged("scene");
	//    }
	//}

	//[XmlElement("extra")]
	//public List<Extra> extra
	//{
	//    get
	//    {
	//        return this.extraField;
	//    }
	//    set
	//    {
	//        this.extraField = value;
	//        this.RaisePropertyChanged("extra");
	//    }
	//}


}