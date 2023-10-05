(define
	(domain gripper-strips)

	(:types
		room
		ball
		gripper
	)

	(:predicates
		(at-robby ?r - room)
		(at ?b - ball ?r - room)
		(free ?g - gripper)
		(carry ?o - ball ?g - gripper)
		(leader-state-at-robby ?r - room)
		(leader-state-at ?b - ball ?r - room)
		(leader-state-free ?g - gripper)
		(leader-state-carry ?o - ball ?g - gripper)
		(is-goal-at-robby ?r - room)
		(is-goal-at ?b - ball ?r - room)
		(is-goal-free ?g - gripper)
		(is-goal-carry ?o - ball ?g - gripper)
		(leader-turn)
	)

	(:action fix_move
		:parameters ( ?from - room ?to - room)
		:precondition 
			(and
				(leader-state-at-robby ?from)
				(leader-turn)
			)
		:effect 
			(and
				(leader-state-at-robby ?to)
				(not
					(leader-state-at-robby ?from)
				)
			)
	)

	(:action attack_move
		:parameters ( ?from - room ?to - room)
		:precondition 
			(and
				(at-robby ?from)
				(not
					(leader-turn)
				)
			)
		:effect 
			(and
				(at-robby ?to)
				(not
					(at-robby ?from)
				)
			)
	)

	(:action attack_move_goal
		:parameters ( ?from - room ?to - room)
		:precondition 
			(and
				(at-robby ?from)
				(not
					(leader-turn)
				)
				(leader-state-at-robby ?to)
			)
		:effect 
			(and
				(at-robby ?to)
				(not
					(at-robby ?from)
				)
				(is-goal-at-robby ?to)
			)
	)

	(:action fix_pick
		:parameters ( ?obj - ball ?room - room ?gripper - gripper)
		:precondition 
			(and
				(leader-state-at ?obj ?room)
				(leader-state-at-robby ?room)
				(leader-state-free ?gripper)
				(leader-turn)
			)
		:effect 
			(and
				(leader-state-carry ?obj ?gripper)
				(not
					(leader-state-at ?obj ?room)
				)
				(not
					(leader-state-free ?gripper)
				)
			)
	)

	(:action attack_pick
		:parameters ( ?obj - ball ?room - room ?gripper - gripper)
		:precondition 
			(and
				(at ?obj ?room)
				(at-robby ?room)
				(free ?gripper)
				(not
					(leader-turn)
				)
			)
		:effect 
			(and
				(carry ?obj ?gripper)
				(not
					(at ?obj ?room)
				)
				(not
					(free ?gripper)
				)
			)
	)

	(:action attack_pick_goal
		:parameters ( ?obj - ball ?room - room ?gripper - gripper)
		:precondition 
			(and
				(at ?obj ?room)
				(at-robby ?room)
				(free ?gripper)
				(not
					(leader-turn)
				)
				(leader-state-carry ?obj ?gripper)
			)
		:effect 
			(and
				(carry ?obj ?gripper)
				(not
					(at ?obj ?room)
				)
				(not
					(free ?gripper)
				)
				(is-goal-carry ?obj ?gripper)
			)
	)

	(:action fix_drop
		:parameters ( ?obj - ball ?room - room ?gripper - gripper)
		:precondition 
			(and
				(leader-state-carry ?obj ?gripper)
				(leader-state-at-robby ?room)
				(leader-turn)
			)
		:effect 
			(and
				(leader-state-at ?obj ?room)
				(leader-state-free ?gripper)
				(not
					(leader-state-carry ?obj ?gripper)
				)
			)
	)

	(:action attack_drop
		:parameters ( ?obj - ball ?room - room ?gripper - gripper)
		:precondition 
			(and
				(carry ?obj ?gripper)
				(at-robby ?room)
				(not
					(leader-turn)
				)
			)
		:effect 
			(and
				(at ?obj ?room)
				(free ?gripper)
				(not
					(carry ?obj ?gripper)
				)
			)
	)

	(:action attack_drop_goal
		:parameters ( ?obj - ball ?room - room ?gripper - gripper)
		:precondition 
			(and
				(carry ?obj ?gripper)
				(at-robby ?room)
				(not
					(leader-turn)
				)
				(leader-state-at ?obj ?room)
				(leader-state-free ?gripper)
			)
		:effect 
			(and
				(at ?obj ?room)
				(free ?gripper)
				(not
					(carry ?obj ?gripper)
				)
				(is-goal-at ?obj ?room)
				(is-goal-free ?gripper)
			)
	)



	(:action fix_pass-turn
		:parameters ()
		:precondition 
			(leader-turn)
		:effect 
			(and
				(not
					(leader-turn)
				)
				(forall ( ?r - room)
					(when
						(not
							(leader-state-at-robby ?r)
						)
						(is-goal-at-robby ?r)
					)

				)

				(forall ( ?b - ball ?r - room)
					(when
						(not
							(leader-state-at ?b ?r)
						)
						(is-goal-at ?b ?r)
					)

				)

				(forall ( ?g - gripper)
					(when
						(not
							(leader-state-free ?g)
						)
						(is-goal-free ?g)
					)

				)

				(forall ( ?o - ball ?g - gripper)
					(when
						(not
							(leader-state-carry ?o ?g)
						)
						(is-goal-carry ?o ?g)
					)

				)

			)
	)

	(:action attack_reach-goal
		:parameters ()
		:precondition 
			(leader-turn)
		:effect 
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
