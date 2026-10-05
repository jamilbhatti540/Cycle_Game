/* 
 * This scripts demonstrates how lane data available through the scripting API can be used
 * 
 * Minimum requirements: EasyRoads3D Pro v3.2
 * 
 * Current Limitations:
 * 
 * -	This example does not take crossing lane connectors with no roads attached into consideration.
 *    	The vehicle will hang. All intersections in the demo scene only have lane connectors with roads attached 
 * 
 * */

using UnityEngine;
using System.Collections;
using EasyRoads3Dv3; 

public enum ERVehicleLocation
{
	road, 
	crossing 
};

public class ERVehicle : MonoBehaviour {

	public ERRoad road;
	private ConnectedTo connectedTo;
	[HideInInspector]
	public int lane;
	[HideInInspector]
	public GameObject go;
	[HideInInspector]
	public int currentElement = 0;
	private float t = 0;
	private float distance = 0;
	private Vector3 oldPosition;
	public float speed = 0f;
	public float targetSpeed = 10f;
	public float accelerator = 1f;
	public float decelerator = 1f;
	public float profile = 5;
	private float fSpeed = 0;
	private Vector3 pos;
	[HideInInspector]
	public Vector3[] points;
	// Is the car driving on the left or right side
	[HideInInspector]
	public ERLaneDirection laneDirection;
	// Is the car on a road or on a crossing
	[HideInInspector]
	public ERVehicleLocation ERVehicleLocation;

	// Use this for initialization
	void Start () {
		go = gameObject;
	}
	
	// Update is called once per frame
	void Update () {

		float deltaT = Time.deltaTime;
		if(speed != targetSpeed){
			if(speed < targetSpeed){
				// accelerate to target speed
				speed = Mathf.Lerp(speed, targetSpeed, deltaT * accelerator); // 0.5..5
			}else{
				// decelerate to target speed 
				speed = Mathf.Lerp(speed, targetSpeed, deltaT * decelerator);// 1..10
			}
		}

		// get the new distance
		fSpeed = (deltaT * speed);
		distance = fSpeed;

		// get the position on the road
		pos = ERMath.GetPosition(points, go.transform.position, ref distance, ref currentElement, ref t);

		if(pos == go.transform.position){
			// The end of the road section or connection is reached
			// Get next lane data

			if(ERVehicleLocation == ERVehicleLocation.road){
				// The vehicle is on a road > get intersection, available connections and lane info
				if(!GetConnectionLane()){
					// no connection at this side of the road > move in opposite direction
					// Get points other side road
					if(laneDirection == ERLaneDirection.Left){
						points = road.GetLanePoints(lane, ERLaneDirection.Right);
						laneDirection = ERLaneDirection.Right;
					}else{
						points = road.GetLanePoints(lane, ERLaneDirection.Left);
						laneDirection = ERLaneDirection.Left;
					}
				}
			}else{

				if(road == null){
					Debug.Log("no road connected to the start of this lane" );
					return;
				}

				// The vehicle is on an intersection, based on the current lane info the road object was already set to the next road 
				// Get lane points depending on whether this road is attached to this intersection at the start or end
				if(connectedTo == ConnectedTo.Start){
					// the intersection connects to the start of this road object (marker 1)	
					if(ERTrafficManager.trafficDirection == ERTrafficDirection.RHT){
						// Right-hand Traffic setup
						// the vehicle will drive on the right side of the road object
						// get lane points at right side of the road
						points = road.GetLanePoints(lane, ERLaneDirection.Right);
						laneDirection = ERLaneDirection.Right;
					}else{
						// Left-hand Traffic setup
						// the vehicle will drive on the left side of the road object
						// get lane points at left side of the road
						points = road.GetLanePoints(lane, ERLaneDirection.Left);
						laneDirection = ERLaneDirection.Left;
					}
				}else{
					// the intersection connects to the end of this road object (the last marker)
					if(ERTrafficManager.trafficDirection == ERTrafficDirection.RHT){
						// Right-hand Traffic setup
						// the vehicle will drive on the left side of the road object
						// get lane points at left side of the road
						points = road.GetLanePoints(lane, ERLaneDirection.Left);
						laneDirection = ERLaneDirection.Left;
					}else{
						// Left-hand Traffic setup
						// the vehicle will drive on the right side of the road object
						// get lane points at right side of the road
						points = road.GetLanePoints(lane, ERLaneDirection.Right);
						laneDirection = ERLaneDirection.Right;
					}
				}

				// Set the target speed based on the road type and vehicle profile
				targetSpeed = (road.GetSpeedLimit());
				// Set the Minimum speed to Speed Limit - 10% for slowest vehicles to Speed Limit + max 10% for fast vehicles  
				targetSpeed += Mathf.Lerp(-targetSpeed * 0.1f, targetSpeed * 0.1f, profile / 10) / 3.6f;

				ERVehicleLocation = ERVehicleLocation.road;
			}
			// reset position variables
			t = 0;
			currentElement = 0;

			// get final position
			pos = ERMath.GetPosition(points, go.transform.position, ref distance, ref currentElement, ref t);
			pos.y += 1;

		}else{

			pos.y += 1;
		}

		// set the vehicle to the new position
		if(points != null){
			go.transform.position = pos;
			go.transform.forward = (pos - oldPosition).normalized;
			oldPosition = pos;
		}
	}

	public bool GetConnectionLane(){
		// the intersection
		ERConnection conn;
		int connectionIndex;

		if(road == null){
			Debug.Log(gameObject.name + ": no road");
			speed = 0;
			return false;
		}

		// get the intersection object
		if(laneDirection == ERLaneDirection.Left){
			// the vehicle is on the left lane
			if(ERTrafficManager.trafficDirection == ERTrafficDirection.RHT){
				// Right-hand Traffic setup
				// get connector and connection index at the start of the road
				conn = road.GetConnectionAtStart(out connectionIndex);
			}else{
				// Left-hand Traffic setup
				// get connector and connection index at the end of the road
				conn = road.GetConnectionAtEnd(out connectionIndex);
			}
		}else{
			// the vehicle is on the right lane 
			if(ERTrafficManager.trafficDirection == ERTrafficDirection.RHT){
				// Right-hand Traffic setup
				// get connector and connection index at the end of the road
				conn = road.GetConnectionAtEnd(out connectionIndex);
			}else{
				// Left-hand Traffic setup
				// get connector and connection index at the start of the road
				conn = road.GetConnectionAtStart(out connectionIndex);
			}
		}

		if(conn == null){
			// no connection available
		//	Debug.Log("No Connection - road: " + road.GetName());
			return false;
		}

		// get available lane connectors for the current lane on this connection index
		// lane is set to 0 in this example, we only have 2 lane roads with 1 lane per direction > lane index 0
		ERLaneConnector[]  laneConnectors =  conn.GetLaneData(connectionIndex, lane);

		if(laneConnectors.Length == 0){
			Debug.Log("0 connections:" + conn.name);
			return false;
		}

		// randomly select a lane connector from laneConnectors
		ERLaneConnector laneConnector = laneConnectors[Random.Range(0, laneConnectors.Length)];

        // Update the lane index to handle possible lane changes
        lane = laneConnector.endLaneIndex;

        // Get lane points from this lane connector
        points = laneConnector.points;

		// Set the target speed based on the Speed Limit for this connector and vehicle profile
		targetSpeed = laneConnector.speedLimit;

		// Set the Minimum speed to Speed Limit - 10% for slowest vehicles to Speed Limit + max 10% for fast vehicles  
		// divide by 3.6 for for km/hour
		targetSpeed += Mathf.Lerp(-targetSpeed * 0.1f, targetSpeed * 0.1f, profile / 10) / 3.6f;

		//a void low speed
		if(targetSpeed < 3)targetSpeed = 3;

		// get the next road object attached to the end of this connection 
		road = conn.GetConnectedRoad(laneConnector.endConnectionIndex, out connectedTo);

		// set current vehicaly location to crossing
		ERVehicleLocation = ERVehicleLocation.crossing;

		return true;
	}

}
