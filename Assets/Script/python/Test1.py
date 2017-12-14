import UnityEngine as u

class Test1(): 
    def Awake(self, this):
        u.Debug.Log("Awake Method")
        pass

    def Start(self, this):
        u.Debug.Log("Start Method")
        pass

    def Update(self, this):
    	this.transform.Rotate(u.Vector3(2,2,1))