using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using SFB;

public class Full_Buttons : MonoBehaviour
{
    public UnityEngine.UI.Dropdown dropdown;
    public List<Vector3> positions;
    public GameObject target;
    public bool point_selected;
    public Vector3 selected_point;
    public GameObject selectedDuplicateTarget;
    public List<GameObject> DuplicateTargets;
    public UnityEngine.UI.Toggle ShowAllPointsToggle;
    List<string> m_DropOptions = new List<string>(); // temp list of drop options (for editing the dropdown menu)
    GameObject joint; //refers to the JointAngleFB input field
    List<string> jointAngles = new List<string>(); //keeps track of the robotic joint angles at each position
    private double start_pos; // start position where the target populates
    private double end_pos; // end position where the target populates
    private int n_steps; // temp number of steps from start to end
    private const int n_past = 5; // fixed number of steps from start to end
    private int to_generate = 10; // fixed number of iterations to run data generation
    private int generating; // temp number of iterations to run data generation
    private double distance; // distance per step

    void Start()
    {
        ShowAllPointsToggle = GameObject.Find("ShowAllPointsToggle").GetComponent<UnityEngine.UI.Toggle>();
        dropdown = GameObject.Find("Dropdown").GetComponent<UnityEngine.UI.Dropdown>();
        target = GameObject.Find("Target");
        positions = target.GetComponent<Mouse_drag>().positions;
        point_selected = false;
        DuplicateTargets = new List<GameObject>();
        joint = GameObject.Find("JointAngleFB");

        end_pos = UnityEngine.Random.Range((float)0, (float)0.8);
        start_pos = UnityEngine.Random.Range((float)-0.7, (float)0);
        distance = (end_pos - start_pos) / n_past;
        n_steps = n_past;
        generating = to_generate;
    }

    ///<summary>
    ///Refreshes the numbering on the dropdown menu
    ///</summary>
    private void RefreshDropdownNumbering()
    {
        for (int i = 0; i < dropdown.options.Count; i++)  //Go through each dropdown option
        {
            dropdown.options[i].text = (i + 1).ToString() + ": " + dropdown.options[i].text.Split(':')[1]; //refresh the numbering on the dropdown
        }
    }

    private int frames = 0; // frames to pass before updating

    void Update()
    {
        if (generating != to_generate){
            frames++;
            if (frames % 400 == 0 && n_steps != n_past && start_pos < end_pos) {
                StartCoroutine(AutoMove());
            }

            if (frames % 400 == 0 && n_steps == n_past){
                Save();
                generating += 1;
                //reset variables for generating data
                start_pos = UnityEngine.Random.Range((float)-0.7, (float)0);
                end_pos = UnityEngine.Random.Range((float)0, (float)0.8);
                distance = (end_pos - start_pos) / n_past;
                n_steps = 0;
            }
        }
    }

    ///<summary>
    ///Moves the target automatically to generate data
    ///</summary>
    IEnumerator AutoMove(){
         Vector3 newVector = new Vector3((float) start_pos, target.transform.position.y, target.transform.position.z);
         target.transform.position = newVector;
         start_pos += distance;
         Add_Point();
         yield return new WaitForSeconds(2); // waits 2 second before recording the joint angle
         string jointAngle = joint.GetComponent<UnityEngine.UI.InputField>().text;
         jointAngles.Add(jointAngle);
         Debug.Log(jointAngle);
         n_steps++;
    }

    ///<summary>
    ///Set up variables to generate data (gets called when generate data button is clicked)
    ///</summary>
    public void GenerateData()
    {
        start_pos = UnityEngine.Random.Range((float)-0.7, (float)0);
        end_pos = UnityEngine.Random.Range((float)0, (float)0.8);
        n_steps = 0;
        generating = 0;
        distance = (end_pos - start_pos) / n_past;
    }

    ///<summary>
    ///Creates a duplicate target sphere and renders it only if boolean render enables it
    ///</summary>
    private GameObject CreateTargetSphere(Vector3 position, bool render)
    {
        GameObject newobj = Instantiate(GameObject.Find("DuplicateTarget")); //Creating a new gameobject
        newobj.tag = "duplicate";
        MeshRenderer[] renderers = newobj.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers) //go through each Mesh Renderer and set the enabled value to the parameter
        {
            renderer.enabled = render;
        }
        newobj.transform.position = position; //set the transform position of the newly created object to the position parameter
        newobj.transform.localScale = target.transform.localScale; //set local scale to correct target scale
        return newobj; //return new object
    }

    ///<summary>
    ///Renders all targets in the duplicate targets array
    ///</summary>
    private void TargetPointFunctionRender(bool on)
    {
        foreach (GameObject newobj in DuplicateTargets) //go through each gameobject in Duplicate Targets
        {
            RenderTarget(newobj, on); //use the render function
        }
    }

    ///<summary>
    ///Renders a target
    ///</summary>
    private void RenderTarget(GameObject obj, bool on)
    {
        MeshRenderer[] renderers = obj.GetComponentsInChildren<MeshRenderer>(); //get the renderers
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.enabled = on; //set the render enabled value to parameter boolean on
        }
    }

    ///<summary>
    ///turns a string in the form: "x,y,z" into a Vector3 and returns the vector;
    ///</summary>
    public Vector3 String_to_Vector3(string val)
    {
        Vector3 retval; //create a vector to return
        string[] values = new string[3];//create a string array to store values of x y and z in string form
        values = val.Split(',');
        //set x, y, and z values from string array
        retval.x = float.Parse(values[0]);
        retval.y = float.Parse(values[1]);
        retval.z = float.Parse(values[2]);

        return retval; // return vector
    }

    ///<summary>
    ///takes in a Vector3 and returns a string in the form: "x,y,z"
    ///</summary>
    public string Vector3_to_String(Vector3 val)
    {
        string retval; // create a string
                       //cocatenate x,y,and z strings
        retval = val.x.ToString("F2") + ",";
        retval += val.y.ToString("F2") + ",";
        retval += val.z.ToString("F2");
        return retval; // return string
    }

    ///<summary>
    ///resets variables to initial state;
    ///</summary>
    public void Reset(){
        dropdown.ClearOptions();
        m_DropOptions = new List<string>();
        positions = new List<Vector3>();
        dropdown.RefreshShownValue();
        jointAngles = new List<string>();
    }

    ///<summary>
    ///Inserts Current Target Sphere position at the end of the positions array and dropdown list
    ///</summary>
    public void Add_Point()
    {
        Vector3 newVector = target.transform.position; //set a new vector holding the current target position
        m_DropOptions.Add((dropdown.options.Count + 1).ToString() + ": " + Vector3_to_String(newVector)); // add the new vector to m_DropOptions
        dropdown.ClearOptions(); // clear the current dropdown menu
        dropdown.AddOptions(m_DropOptions); //add the new list of options from m_DropOptions to dropdown menu
        positions.Add(newVector); // add the location of the target to list of vectors
        dropdown.RefreshShownValue(); //Dropdown refresh shown value
        DuplicateTargets.Add(CreateTargetSphere(newVector, ShowAllPointsToggle.isOn)); //Add a duplicate target to be shown

        if (generating == to_generate){
            string jointAngle = joint.GetComponent<UnityEngine.UI.InputField>().text;
            jointAngles.Add(jointAngle); // add robotic joint angles to the list of joint angles
        }
    }

    ///<summary>
    ///Deletes point currently selected on the dropdown menu
    ///</summary>
    public void Delete_Point()
    {
        //first check to see if positions have a point to be deleted
        if (positions.Count > 1)
        {
            //remove the specified value
            positions.RemoveRange(dropdown.value, 1);
            dropdown.options.RemoveRange(dropdown.value, 1);
            m_DropOptions.RemoveRange(dropdown.value, 1);
            //refresh 
            RefreshDropdownNumbering();
            dropdown.RefreshShownValue();
            //render specified target off
            RenderTarget(DuplicateTargets[dropdown.value], false);
            DuplicateTargets.RemoveRange(dropdown.value, 1);

            jointAngles.RemoveRange(dropdown.value, 1);

        }
        else if (positions.Count == 1) // edge case if there's only one point left
        {
            dropdown.ClearOptions();
            dropdown.RefreshShownValue();
            m_DropOptions = new List<string>();
            jointAngles = new List<string>();

        }
        else
        {
            //print to the console 
            print("No points to delete");
        }
    }

    ///<summary>
    ///Called when the X coordinate edit field value if end edited, edits the value in the positions array and dropdown menu
    ///</summary>
    public void CoordXEdit()
    {
        //get value of the edit field
        string x = GameObject.Find("CoordX").GetComponent<UnityEngine.UI.InputField>().text;

        if (x.Contains("x:")) // if the input field contains placeholder text
        {
            x = x.Remove(0, 2);
        }

        if (float.TryParse(x, out float n))
        {
            //get value of the edit field
            float currValue = float.Parse(x);
            //create new vector with the new X value
            Vector3 newVector = new Vector3(currValue, target.transform.position.y, target.transform.position.z);
            target.transform.position = newVector;
        }
    }

    ///<summary>
    ///Called when the Y coordinate edit field value if end edited, edits the value in the positions array and dropdown menu
    ///</summary>
    public void CoordYEdit()
    {
        //get value of the edit field
        string y = GameObject.Find("CoordY").GetComponent<UnityEngine.UI.InputField>().text;

        if (y.Contains("y:")) // if the input field contains placeholder text
        {
            y = y.Remove(0, 2);
        }

        if (float.TryParse(y, out float n))
        {
            //get value of the edit field
            float currValue = float.Parse(y);
            //create new vector with the new X value
            Vector3 newVector = new Vector3(target.transform.position.x, currValue, target.transform.position.z);
            target.transform.position = newVector;
        }
    }

    ///<summary>
    ///Called when the Z coordinate edit field value if end edited, edits the value in the positions array and dropdown menu
    ///</summary>
    public void CoordZEdit()
    {
        //get value of the edit field
        string z = GameObject.Find("CoordZ").GetComponent<UnityEngine.UI.InputField>().text;

        if (z.Contains("z:")) // if the input field contains placeholder text
        {
            z = z.Remove(0, 2);
        }

        if (float.TryParse(z, out float n))
        {
            //get value of the edit field
            float currValue = float.Parse(z);
            //create new vector with the new Z value
            Vector3 newVector = new Vector3(target.transform.position.x, target.transform.position.y, currValue);

            target.transform.position = newVector;
        }
    }

    ///<summary>
    ///Called when the dropdown menu value is changed: changes the x,y,and z edit fields and changes the target positions
    ///</summary>
    public void Change_target_position()
    {
        //get the vector that the dropdown menu now has
        Vector3 pos = positions[dropdown.value];
        //set target position to the position
        target.transform.position = pos;
        //update coordinate edit fields
        GameObject.Find("CoordX").GetComponent<UnityEngine.UI.InputField>().text = pos.x.ToString("F2");
        GameObject.Find("CoordY").GetComponent<UnityEngine.UI.InputField>().text = pos.y.ToString("F2");
        GameObject.Find("CoordZ").GetComponent<UnityEngine.UI.InputField>().text = pos.z.ToString("F2");
    }

    ///<summary>
    ///Inserts the target sphere position as a point prior to the point selected on the dropdown menu
    ///</summary>
    public void Insert_Point_Before()
    {
        //Get position of the target
        Vector3 newVector = target.transform.position;
        //First check if a point is selected
        if (point_selected)
        {
            //set newVector equal to the selected point
            newVector = selected_point;
            //Insert DuplicateTarget into the correct position
            DuplicateTargets.Insert(dropdown.value, selectedDuplicateTarget);
        }
        else
        {
            //Insert a newly created Duplicate target
            DuplicateTargets.Insert(dropdown.value, CreateTargetSphere(newVector, ShowAllPointsToggle.isOn));
        }
        //Insert newVector into positions array
        positions.Insert(dropdown.value, newVector);
        //Update dropdown to include new point
        dropdown.options.Insert(dropdown.value, new UnityEngine.UI.Dropdown.OptionData(":" + Vector3_to_String(newVector)));
        //Render the newly inserted target if the toggle is on
        RenderTarget(DuplicateTargets[dropdown.value], ShowAllPointsToggle.isOn);
        //refresh interface dropdown
        RefreshDropdownNumbering();
        dropdown.RefreshShownValue();
        //Change point_selected to false since if a point is selected it has now been inserted or if there is no point selected
        //then it is already false
        point_selected = false;

    }

    ///<summary>
    ///Inserts the target sphere position as a point after to the point selected on the dropdown menu
    ///</summary>
    public void Insert_Point_After()
    {
        //Get position of the target
        Vector3 newVector = GameObject.Find("Target").transform.position;
        //First check if a point is selected
        if (point_selected)
        {
            //set newVector equal to the selected point
            newVector = selected_point;
            //Insert DuplicateTarget into the value+1 position to insert after
            DuplicateTargets.Insert(dropdown.value + 1, selectedDuplicateTarget);
        }
        else
        {
            DuplicateTargets.Insert(dropdown.value + 1, CreateTargetSphere(newVector, ShowAllPointsToggle.isOn));
        }
        //Insert newVector into positions array
        positions.Insert(dropdown.value + 1, newVector);
        //Update dropdown to include new point and Duplicate target to include new point
        dropdown.options.Insert(dropdown.value + 1, new UnityEngine.UI.Dropdown.OptionData(":" + Vector3_to_String(newVector)));
        DuplicateTargets.Insert(dropdown.value + 1, selectedDuplicateTarget);
        //update dropdown value
        dropdown.value = dropdown.value + 1;
        //Render the newly inserted target if the toggle is on
        RenderTarget(DuplicateTargets[dropdown.value + 1], ShowAllPointsToggle.isOn);
        //refresh interface dropdown
        RefreshDropdownNumbering();
        dropdown.RefreshShownValue();
        //Change point_selected to false since if a point is selected it has now been inserted or if there is no point selected
        //then it is already false
        point_selected = false;
    }

    ///<summary>
    ///Loads a .txt file with vectors into the positions array and the dropdown menu, file needs to be in the form: x,y,z on each line
    ///</summary>
    public void Load()
    {
        //create a path string that starts at the data path and starts with save.txt
        // string[] path = StandaloneFileBrowser.OpenFilePanel("", Application.dataPath, "", false);

        // read file into an array of strings with each entry being a different line
        string[] strings_vector = File.ReadAllLines("../RobotLearningInterface/Assets/Save.csv");
        //reset interface
        DuplicateTargets.Clear();
        dropdown.ClearOptions();
        positions.Clear();
        GameObject.Find("CoordX").GetComponent<UnityEngine.UI.InputField>().text = "";
        GameObject.Find("CoordY").GetComponent<UnityEngine.UI.InputField>().text = "";
        GameObject.Find("CoordZ").GetComponent<UnityEngine.UI.InputField>().text = "";
        //For each string in the string array
        for (int i = 1; i < strings_vector.GetLength(0); i++)
        {
            //get the vector split from the numbering
            string[] newString = strings_vector[i].Split(',');

            //parse line into coordinates
            float x = float.Parse(newString[1]);
            float y = float.Parse(newString[2]);
            float z = float.Parse(newString[3]);
            Vector3 newVector = new Vector3(x, y, z);

            //add point to the interface
            positions.Add(newVector);
            dropdown.options.Add(new UnityEngine.UI.Dropdown.OptionData((i).ToString() + ": " + Vector3_to_String(newVector)));
            DuplicateTargets.Add(CreateTargetSphere(newVector, ShowAllPointsToggle.isOn));
        }
        //refresh dropdown menu
        dropdown.RefreshShownValue();
    }

    ///<summary>
    ///Saves a .txt file to application.dataPath with a vector in the form of "x,y,z" on each line 
    ///</summary>
    public void Save()
    {
        //start path as the save.csv and joints.csv in the application's datapath (will appear under assets)
        string path = Application.dataPath + "/Save.csv";
        string jointPath = Application.dataPath + "/Joints.csv";

        File.WriteAllText(path, "");
        File.WriteAllText(jointPath, "");

        StreamWriter writer = new StreamWriter(path, true);
        StreamWriter jointWriter = new StreamWriter(jointPath, true);

        writer.WriteLine(",x,y,z"); // write header

        for (int i = 0; i < dropdown.options.Count; i++)
        {
            // write each postion as a line to the csv
            string content = dropdown.options[i].text.Replace(": ", ",");
            // File.AppendAllText(path, content);
            writer.WriteLine(content);
        }
        writer.Close();

        jointWriter.WriteLine(",1,2,3,4,5,6"); //write header

        for (int i = 0; i < jointAngles.Count; i++)
        {
            // write each joint angle as a line to the csv
            string content = jointAngles[i].Replace(")", "").Replace(" ", "").Replace("(", (i + 1) + ",");
            jointWriter.WriteLine(content);
        }
        jointWriter.Close();

    }

    ///<summary>
    ///Selects a point and takes it out of the dropdown menu
    ///</summary>
    public void SelectPoint()
    {
        //First check if a point is selected or not
        if (!point_selected) //If a point is not selected, select the point at the current dropdown menu value
        {
            //Set global values for selected point
            point_selected = true;
            selected_point = positions[dropdown.value];
            selectedDuplicateTarget = DuplicateTargets[dropdown.value];
            // Turn the Duplicate Target sphere off
            RenderTarget(DuplicateTargets[dropdown.value], false);
            dropdown.options.RemoveRange(dropdown.value, 1);
            DuplicateTargets.RemoveRange(dropdown.value, 1);
            RefreshDropdownNumbering();
            dropdown.RefreshShownValue();
        }
        if (point_selected) //If a point is selected let the user know a point is already selected
        {
            print("Point Already Selected:");
            print(selected_point);
        }

    }
    ///<summary>
    ///Rotates the camera around the robot by 10 degrees
    ///</summary>
    public void Rotate_Camera()
    {
        //Rotate the CameraRotater's transform by 10 degrees in the 10 degrees
        GameObject.Find("CameraRotater").transform.Rotate(0, 10, 0);
    }
    public void ShowPoints()
    {
        //Render all points if the toggle is On for Showing all Points
        TargetPointFunctionRender(ShowAllPointsToggle.isOn);
    }
}
