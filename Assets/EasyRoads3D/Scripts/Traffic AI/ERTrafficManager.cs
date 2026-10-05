using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyRoads3Dv3;

public class ERTrafficManager : MonoBehaviour {

	// the vehicle object with the ERVehicle component attached
	public GameObject vehicle;
	static public ERTrafficDirection trafficDirection;

	// Use this for initialization
	void Start () {

		if (vehicle == null)
		{
			Debug.LogWarning("EasyRoads3D Warning: A vehicle object needs to be assigned to the Traffic Manager component");
		}

		// Make sure the ERVehicle component is attached to the vehicle object
		if (vehicle.GetComponent<ERVehicle>() == null) vehicle.AddComponent<ERVehicle>();

		// get a reference to the road network
		ERRoadNetwork roadNetwork = new ERRoadNetwork();
		// get all roads in the road network
		// this will be used to instantiate ERVehicle objects on each road
		ERRoad[] roads = roadNetwork.GetRoadObjects();
		int i = 0;
		foreach(ERRoad road in roads){
			if (!road.IsSideObject()) {
				i++;
				GameObject go = Instantiate(vehicle);
				go.name = "Vehicle " + i;
				ERVehicle scr = go.GetComponent<ERVehicle>();

				// set the location for this object
				scr.road = road;
				scr.ERVehicleLocation = ERVehicleLocation.road;
				scr.laneDirection = ERLaneDirection.Left;

				// create a profile for this object  
				scr.profile = Random.Range(1, 10);
				scr.accelerator = Mathf.Lerp(0.5f, 5f, scr.profile / 10);
				scr.decelerator = Mathf.Lerp(1f, 30f, scr.profile / 10);

				// get lane count


				// set the current lane index and lane data for this object
				int lanes = scr.road.GetLeftLaneCount();
				scr.lane = Random.Range(0, lanes);
				
				try
				{
					scr.points = road.GetLanePoints(scr.lane, ERLaneDirection.Left);
					scr.currentElement = Random.Range(3, scr.points.Length - 3);
				}
				catch
				{
					Debug.Log(road.roadScript.roadName);
					Destroy(go);
					continue;
				}

				// Set the target speed based on the road type and vehicle profile
				scr.targetSpeed = road.GetSpeedLimit();
				// Set the Minimum speed to Speed Limit - 10% for slowest vehicles to Speed Limit + max 10% for fast vehicles
				// divide by 3.6 for for km/hour
				scr.targetSpeed += Mathf.Lerp(-scr.targetSpeed * 0.1f, scr.targetSpeed * 0.1f, scr.profile / 10) / 3.6f;


				//Debug.Log("Road Speed Limit: " + road.GetSpeedLimit() +" target speed" + scr.targetSpeed);
			}
		}

		// get the traffic direction, Left or Right hand Traffic
		trafficDirection = roadNetwork.GetTrafficDirection(); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
