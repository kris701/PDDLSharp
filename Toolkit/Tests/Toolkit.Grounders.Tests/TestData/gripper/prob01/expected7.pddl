(:action pick
	:parameters (ball1 roomb left)
	:precondition  
		(and  
			(ball ball1) 
			(room roomb) 
			(gripper left)
			(at ball1 roomb) 
			(at-robby roomb) 
			(free left)
		)
	:effect 
		(and 
			(carry ball1 left)
		    (not (at ball1 roomb)) 
		    (not (free left))
		)
	)