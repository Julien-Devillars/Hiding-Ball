# Hiding-Ball



### Summary

2 balls, one is emmiting light, the second one is hiding from the light.

### Creating the Ground

First I create a Ground with the width, height and size that I want :

- 8x8 Size 1 :       ![image](./Images/8x8_Ground.png)
- 20x20 Size 0.5 :![image](./Images/20x20_Ground.png)
- 40x40 Size : 0.25 :![image](./Images/40x40_Ground.png)



### Creating wall and obstacles

The idea here is to add with a simple click, cubes, to create shadows due to the red ball which is representing a light. It could be interesting to remove cube so I added this feature :



* **Left click** add cube
* **Right click** remove cube

![image](./Images/WallAdded.png)



### Quadtree and shadow

The best way to get the shadows would be to use shadowmap, but it's painful to use and I want to implement it with another way. 

We will use **Quadtree**. Why ?

* Quadtree is fast.
* Quadtree is efficient. 
* I can handle the number of subdivision to have a better precision.
* It avoid a lot of useless calculation.

<img src="./Images/Shadows.png" alt="image" style="zoom:50%;" />   <img src="./Images/Shadows_quadtree_sub2.png" alt="image" style="zoom:50%;" />   <img src="./Images/Shadows_quadtree_sub4.png" alt="image" style="zoom:50%;" />  <img src="./Images/Shadows_quadtree_sub6.png" alt="image" style="zoom:50%;" />



Our way to do it (using ray cast and if we are touching a wall, we are in shadow) is not the best, but it's fast and do the job.

 

We can improve our method by avoiding to go into the square that are "fully" in shadow.



<img src="./Images/Shadows.png" alt="image" style="zoom:50%;" />  <img src="./Images/Shadows_quadtree_faster_sub2.png" alt="image" style="zoom:50%;" />  <img src="./Images/Shadows_quadtree_faster_sub4.png" alt="image" style="zoom:50%;" />  <img src="./Images/Shadows_quadtree_faster_sub6.png" alt="image" style="zoom:50%;" />    



That way we can even use subdivion 8 (which is taking a bit of time previously ~8s to around ~1s now). It's taking that much time to draw the "debug" lines. Without take it takes less time.

 

![image](./Images/Shadows_quadtree_faster_sub8.png)





### Merging the shadow as area

