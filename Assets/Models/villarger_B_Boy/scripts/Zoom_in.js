#pragma strict
var zoom : int = 22;
var normal : int = 40;
var smooth : float = 5;
private var isZoomed = false;
function Update () {
     if(Input.GetMouseButtonDown(1)){
          isZoomed = !isZoomed; 
     }
 
     if(isZoomed == true){
          GetComponent.<Camera>().fieldOfView = Mathf.Lerp(GetComponent.<Camera>().fieldOfView,zoom,Time.deltaTime*smooth);
     }
     else{
        GetComponent.<Camera>().fieldOfView = Mathf.Lerp(GetComponent.<Camera>().fieldOfView,normal,Time.deltaTime*smooth);
     }
}