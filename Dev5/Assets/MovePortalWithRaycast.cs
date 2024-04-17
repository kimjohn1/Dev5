using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Threading;

using UnityEngine.XR;

public class MovePortalWithRaycast : MonoBehaviour
{
    public XRRayInteractor rayInteractor;
    public GameObject objectToMove;
    public GameObject fixedObject;
    public XRNode rightInputSource;
    public XRNode leftInputSource;
    private float scaleSpeed = 0.5f;
    private float rotationSpeed = 45f;
    private bool isOnTable = true;

    private bool portalToggle = false;

    void Update()
    {
        InputDevice rightDevice = InputDevices.GetDeviceAtXRNode(rightInputSource);
        InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(leftInputSource);
        rightDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
        bool isTriggerPressed = triggerValue > 0.1f;

        leftDevice.TryGetFeatureValue(CommonUsages.trigger, out float leftTriggerValue);
        bool leftIsTriggerPressed = leftTriggerValue > 0.1f;
        leftDevice.TryGetFeatureValue(CommonUsages.grip, out float leftGrabValue);
        bool leftIsGrabPressed = leftGrabValue > 0.1f;

        //Toggle Portal Mode
        if (leftIsTriggerPressed && leftIsGrabPressed)
        {
            portalToggle = !portalToggle;
            if (!portalToggle)
            {
            objectToMove.SetActive(portalToggle); fixedObject.SetActive(portalToggle);
            }
            Thread.Sleep(500);
        }

        if (isTriggerPressed && portalToggle)
        {
            objectToMove.SetActive(portalToggle); fixedObject.SetActive(portalToggle);
            RaycastHit hit;
            if (rayInteractor.TryGetCurrent3DRaycastHit(out hit))
            {
                Vector3 modifiedHitPoint = hit.point;
                
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Table"))
                {
                    isOnTable = true;
                    modifiedHitPoint.y = 1 - (1 - objectToMove.transform.localScale.y);
                }
                else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    isOnTable = false;
                    modifiedHitPoint.y = -(1 - objectToMove.transform.localScale.y);
                }
                
                //Move both the exit portal and the entry portal to new locations
                objectToMove.transform.position = modifiedHitPoint;
                XROrigin xrOrigin = FindObjectOfType<XROrigin>();
                fixedObject.transform.position = xrOrigin.transform.position;                
                fixedObject.transform.rotation = xrOrigin.transform.rotation;

                //Set the entry portal slightly away from the player
                Vector3 modifiedFixedPoint = (xrOrigin.transform.forward * 0.5f) + xrOrigin.transform.position;
                fixedObject.transform.position = modifiedFixedPoint;
            }
        }

        rightDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool isAPressed);
        rightDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isBPressed);
        leftDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool isXPressed);
        leftDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isYPressed);

        if (isAPressed && portalToggle)
        {
            objectToMove.transform.localScale = objectToMove.transform.localScale - Vector3.one * scaleSpeed * Time.deltaTime;
            Vector3 newPosition = objectToMove.transform.position;
            if (isOnTable)
            {
                newPosition.y = 1 - (1 - objectToMove.transform.localScale.y);
            }
            else
            {
                newPosition.y = -(1 - objectToMove.transform.localScale.y);
            }
            objectToMove.transform.position = newPosition;
        }
        if (isBPressed && portalToggle)
        {
            objectToMove.transform.localScale = objectToMove.transform.localScale + Vector3.one * scaleSpeed * Time.deltaTime;
            Vector3 newPosition = objectToMove.transform.position;
            if (isOnTable)
            {
                newPosition.y = 1 - (1 - objectToMove.transform.localScale.y);
            }
            else
            {
                newPosition.y = -(1 - objectToMove.transform.localScale.y);
            }
            objectToMove.transform.position = newPosition;
        }
        if (isXPressed && portalToggle)
        {
            objectToMove.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        if (isYPressed && portalToggle)
        {
            objectToMove.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
    }
}
