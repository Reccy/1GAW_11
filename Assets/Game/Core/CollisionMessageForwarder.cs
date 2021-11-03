using UnityEngine;

public class CollisionMessageForwarder : MonoBehaviour
{
    [SerializeField] GameObject m_target;

    private void OnTriggerEnter(Collider other)
    {
        m_target.SendMessage("OnTriggerEnter", other, SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerStay(Collider other)
    {
        m_target.SendMessage("OnTriggerStay", other, SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerExit(Collider other)
    {
        m_target.SendMessage("OnTriggerExit", other, SendMessageOptions.DontRequireReceiver);
    }

    private void OnCollisionEnter(Collision collision)
    {
        m_target.SendMessage("OnCollisionEnter", collision, SendMessageOptions.DontRequireReceiver);
    }

    private void OnCollisionStay(Collision collision)
    {
        m_target.SendMessage("OnCollisionStay", collision, SendMessageOptions.DontRequireReceiver);
    }

    private void OnCollisionExit(Collision collision)
    {
        m_target.SendMessage("OnCollisionExit", collision, SendMessageOptions.DontRequireReceiver);
    }
}
