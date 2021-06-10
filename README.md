"# Hiding-Ball" 

2 balls, one is emmiting light, the second one is hiding from the light.

* First I create a Ground with the width, height and size that I want
8x8 Size 1 :
![image](https://user-images.githubusercontent.com/45456710/121600443-bf3d8900-ca44-11eb-92b2-1aacaf200cc1.png)

20x20 Size 0.5 :
![image](https://user-images.githubusercontent.com/45456710/121600462-c795c400-ca44-11eb-851a-bdd6d1972f55.png)

40x40 Size 0.25 :
![image](https://user-images.githubusercontent.com/45456710/121600492-d41a1c80-ca44-11eb-8c7f-b3b3f4d5f5b4.png)

* Second I able to create and remove walls
* * Left click add cube
* * Right click remove cube

![image](https://user-images.githubusercontent.com/45456710/121600768-45f26600-ca45-11eb-8b3b-967e8a327784.png)

* Next step is to know where is shadows, I could use some shadowmap but I want to check for an easier solution. I will use quadtree instead.
* * Quadtree is a fast way to know which position are into the shadow. 
* * I can handle the number of subdivision to have a better precision.
* * It avoid a lot of useless calculation

![image](https://user-images.githubusercontent.com/45456710/121602756-13963800-ca48-11eb-8420-19be7dd0daf9.png)
![image](https://user-images.githubusercontent.com/45456710/121602775-18f38280-ca48-11eb-959b-106457bf80a4.png)


* Next step is to have 
