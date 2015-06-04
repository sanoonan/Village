#pragma strict


var aTexture : Texture;

function OnGUI() {    

if(!aTexture){        

Debug.LogError("Assign a Texture in the inspector.");        
return;    
}    
 
GUI.DrawTexture(Rect(850,540,80,80), aTexture);

}


