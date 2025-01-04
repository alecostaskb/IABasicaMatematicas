using UnityEngine;

// A very simplistic car driving on the x-z plane.

public class Drive : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;

    public GameObject destino;
    private bool autoPilot = false;

    [SerializeField] private float tankSpeed = 10.0f;
    [SerializeField] private float tankRotationSpeed = 0.1f;

    private void Start()
    {
    }

    private void LateUpdate()
    {
        // Get the horizontal and vertical axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

        // Make it move 10 meters per second instead of 10 meters per frame...
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        // Move translation along the object's z-axis
        transform.Translate(0, translation, 0);

        // Rotate around our y-axis
        transform.Rotate(0, 0, -rotation);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CalculateDistance();

            CalculateAngle();
        }

        // si se pulsa T activar/desactivar el autopilot
        if (Input.GetKeyDown(KeyCode.T))
        {
            autoPilot = !autoPilot;
        }

        // si el tanque está a una distancia menor de 3 del objetivo, pararlo
        if (CalculateDistance() < 3)
        {
            autoPilot = false;
        }

        // si el autopilot está activo, ejecutar
        if (autoPilot)
        {
            AutoPilot();
        }
    }

    private void AutoPilot()
    {
        CalculateAngle();

        transform.position += tankSpeed * Time.deltaTime * transform.up;
    }

    private float CalculateDistance()
    {
        // distancia por Pitágoras
        float distance = Mathf.Sqrt(Mathf.Pow(destino.transform.position.x - transform.position.x, 2)
                                    + Mathf.Pow(destino.transform.position.y - transform.position.y, 2)
                                    + Mathf.Pow(destino.transform.position.z - transform.position.z, 2));

        //Debug.Log("<" + transform.name.ToUpper() + "> está a " + distance + " de <" + destino.name.ToUpper() + ">");
        
        // --------------

        //// distancia Unity
        //float uDistance = Vector3.Distance(destino.transform.position, transform.position);

        //Debug.Log("Distancia: " + distance);
        //Debug.Log("U Distancia: " + uDistance);

        //// las distancias son diferentes porque
        //// la primera: this distance is only in two dimensions X and Z.
        //// la segunda: this distance is calculating the distance in three dimensions
        ////             y la coordenada Y es diferente en los objetos

        //// para que la posición Y no afecte a la distancia Unity
        //Vector3 destinoPos = new Vector3(destino.transform.position.x, 0, destino.transform.position.z);
        //Vector3 tankPos = new Vector3(transform.position.x, 0, transform.position.z);

        //// distancia Unity
        //float uDistance2 = Vector3.Distance(destinoPos, tankPos);

        //Debug.Log("U Distancia 2: " + uDistance2);

        //// otra manera de calcular la distancia
        //Vector3 tankToFuel = destinoPos - tankPos;

        //Debug.Log("V magnitude: " + tankToFuel.magnitude);

        //// la magnitud cuadrada es la magnitud al cuadrado
        //// es más rápida, ya que no hace la raiz cuadrada
        //// en vez de raiz((x2-x1)2 + (y2-y1)2 + (z2-z1)2)
        //// hace (x2-x1)2 + (y2-y1)2 + (z2-z1)2
        //Debug.Log("V sqr magnitude: " + tankToFuel.sqrMagnitude);

        // --------------

        return distance;
    }

    private void CalculateAngle()
    {
        Vector3 tankFordward = transform.up;
        Vector3 destinoDirecction = destino.transform.position - transform.position;

        Debug.DrawRay(transform.position, tankFordward * 10, Color.green, 2);
        Debug.DrawRay(transform.position, destinoDirecction, Color.red, 2);

        float dot = tankFordward.x * destinoDirecction.x + tankFordward.y * destinoDirecction.y;
        float angle = Mathf.Acos(dot / (tankFordward.magnitude * destinoDirecction.magnitude));

        //Debug.Log("Angle: " + angle * Mathf.Rad2Deg);
        //Debug.Log("Unity angle: " + Vector3.Angle(tankFordward, destinoDirecction));

        // para saber si el tanque debe girar a la derecha o a la izquierda
        int clockwise = 1;

        if (CrossProduct(tankFordward, destinoDirecction).z < 0)
        {
            clockwise = -1;
        }

        // encarar el tanque al objetivo, si el ángulo entre vectores es mayor de 10
        if (angle * Mathf.Rad2Deg > 10)
        {
            transform.Rotate(0, 0, angle * Mathf.Rad2Deg * clockwise * tankRotationSpeed);
        }
    }

    private Vector3 CrossProduct(Vector3 vector1, Vector3 vector2)
    {
        float xMult = vector1.y * vector2.z - vector1.z * vector2.y;
        float yMult = vector1.x * vector2.z - vector1.z * vector2.x;
        float zMult = vector1.x * vector2.y - vector1.y * vector2.x;

        // vector resultante
        return (new Vector3(xMult, yMult, zMult));
    }
}