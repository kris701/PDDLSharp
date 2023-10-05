(define
	(problem strips-gripper-x-1)

	(:domain gripper-strips)

	(:objects
		rooma - room
		roomb - room
		ball4 - ball
		ball3 - ball
		ball2 - ball
		ball1 - ball
		left - gripper
		right - gripper
	)

	(:init
		(at-robby rooma)
		(free left)
		(free right)
		(at ball4 rooma)
		(at ball3 rooma)
		(at ball2 rooma)
		(at ball1 rooma)
		(gripper left)
		(gripper right)
		(leader-state-at-robby rooma)
		(leader-state-free left)
		(leader-state-free right)
		(leader-state-at ball4 rooma)
		(leader-state-at ball3 rooma)
		(leader-state-at ball2 rooma)
		(leader-state-at ball1 rooma)
		(leader-state-gripper left)
		(leader-state-gripper right)
		(is-goal-at-robby rooma)
		(is-goal-free left)
		(is-goal-free right)
		(is-goal-at ball4 rooma)
		(is-goal-at ball3 rooma)
		(is-goal-at ball2 rooma)
		(is-goal-at ball1 rooma)
		(is-goal-gripper left)
		(is-goal-gripper right)
		(leader-turn)
	)

	(:goal
		(and
			(is-goal-at-robby rooma)
			(is-goal-at-robby roomb)
			(is-goal-at ball4 rooma)
			(is-goal-at ball4 roomb)
			(is-goal-at ball3 rooma)
			(is-goal-at ball3 roomb)
			(is-goal-at ball2 rooma)
			(is-goal-at ball2 roomb)
			(is-goal-at ball1 rooma)
			(is-goal-at ball1 roomb)
			(is-goal-free left)
			(is-goal-free right)
			(is-goal-carry ball4 left)
			(is-goal-carry ball4 right)
			(is-goal-carry ball3 left)
			(is-goal-carry ball3 right)
			(is-goal-carry ball2 left)
			(is-goal-carry ball2 right)
			(is-goal-carry ball1 left)
			(is-goal-carry ball1 right)
		)
	)

)
