using UnityEngine;

[System.Serializable]
public class Inspection
{
    //public System.DateTime Date;
    // inspection.Date = new System.DateTime(2025, 11, 27);
    //string dateString = "2025-11-27";
    //DateTime inspectionDate = DateTime.Parse(dateString);

    public int Year;

    public GeneralInspection General;

    public ResistentArmorLayerInspection ResistentArmorLayer;

    public SuperstructureInspection Superstructure;
    
    public InteriorArmorLayerInspection InteriorArmorLayer;
    
    public UnderwaterInspection Underwater;
    
    public PierInspection Pier;
}
