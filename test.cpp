#include<stdio.h>
#pragma warning(disable:4996)
#include<string.h>
#include<time.h>
#include<stdlib.h>

void card_(struct trump m_card[]);
void display_card(struct trump q[]);
struct trump
{
	char shape[3]; 
	int number;
};
struct trump game_card;

class AI
{
public:
	AI(int n,struct trump q[]) 
	{
		this->num = n;
		start_card(q);
		start_show_card();
	}
	trump a_card[54];
	int point;
	int num;
	int hand_number = 7;
	void start_card(struct trump q[]);
	void start_show_card();
	void game(struct trump q[]);
};
void AI::game(struct trump q[])
{
	trump temp;
	printf("\nAI.%d's turn  ", this->num);
	for (int i = 0; i < this->hand_number; i++)
	{
		if (strcmp(this->a_card[i].shape,game_card.shape) == 0)
		{
			if (this->a_card[i].number <= 10 && this->a_card[i].number != 1) { printf("%s%d", this->a_card[i].shape, this->a_card[i].number); }
			else if (this->a_card[i].number > 10 || this->a_card[i].number == 1) { printf("%s%c", this->a_card[i].shape, this->a_card[i].number); }
			strcpy(temp.shape, this->a_card[i].shape);
			temp.number = this->a_card[i].number;
			strcpy(this->a_card[i].shape,"0");
			this->a_card[i].number = 0;
			this->point += 10;
			strcpy(game_card.shape, temp.shape);
			game_card.number = temp.number;
			this->hand_number--;
			break;
		}
	}


}

void AI::start_card(struct trump q[])
{
	int i = (num * 7) - 7;
	for (int j = i ; j < i+7; j++)
	{
		strcpy(this->a_card[j].shape, q[j].shape);
		this->a_card[j].number = q[j].number;
	}
}
void AI::start_show_card()
{
	printf("AI.%d", this->num);
	printf(" ");
	int i = (num * 7) - 7;
	for (int j = i; j < i+7; j++)
	{
		printf("%s", this->a_card[j].shape);
		if (10 < this->a_card[j].number)
			printf("%-2c  ", this->a_card[j].number);
		else
			printf("%-2d  ", this->a_card[j].number);
		if (j % 7 + 1 == 7){printf("\n");}
	}
}
void setting(struct trump q[]) 
{ 
	printf("\n");
	if (q[28].number <= 10 && q[28].number != 1) { printf("시작 패 : %s%d", q[28].shape, q[28].number); }
	else if (q[28].number > 10 || q[28].number == 1) { printf("시작 패 : %s%c", q[28].shape, q[28].number); }
	printf("\n");
	strcpy(game_card.shape, q[28].shape);
	game_card.number = q[28].number;

	for (int j = 0; j < 54; j++)
	{
		if (j < 26)
		{
			strcpy(q[j].shape, q[j + 29].shape);
			q[j].number = q[j + 29].number;
		}
		else
		{
			strcpy(q[j].shape, "0");
			q[j].number = 0;
		}
	}
	printf("\n남은 카드 \n");
	int i;
	for (i = 0; i < 25; i++)
	{
		printf("%s", q[i].shape);
		if (10 < q[i].number) 
			printf("%-2c  ", q[i].number);
		else
			printf("%-2d  ", q[i].number);

		if (i % 13 + 1 == 13)
		{
			printf("\n");
		}
	}
}
int main()
{
	struct trump card[54];
	card_(card);
	AI a(1, card);
	AI b(2, card);
	AI c(3, card);
	AI d(4, card);
	setting(card);

	char stop;
	stop = 'a';
	while (stop!='z')
	{
		a.game(card);
		b.game(card);
		c.game(card);
		d.game(card);
		scanf("%c", &stop);
	}
	return 0;
}

void display_card(struct trump q[])
{
	int i;
	for (i = 0; i < 54; i++)
	{
			printf("%s", q[i].shape);
			if (10 < q[i].number)
				printf("%-2c  ", q[i].number);
			else
				printf("%-2d  ", q[i].number);

		if (i % 13 + 1 == 13)
		{
			printf("\n");
		}
	}
}

void card_(struct trump m_card[])
{
	int person = 0;
	char shape[4][3] = { "♠", "◆", "♥", "♣"};
	int i, j, rnd;
	struct trump temp;
	srand((unsigned)time(NULL));

	for (i = 0; i < 4; i++)                     
	{
		for (j = i * 13; j < i * 13 + 13; j++)
		{
			strcpy(m_card[j].shape, shape[i]);
			m_card[j].number = j % 13 + 1;

			switch (m_card[j].number)
			{
			case 1:
				m_card[j].number = 'A';
				break;
			case 11:
				m_card[j].number = 'J';
				break;
			case 12:
				m_card[j].number = 'Q';
				break;
			case 13:
				m_card[j].number = 'K';
				break;
			}
		}
	}
	strcpy(m_card[52].shape, "J");
	m_card[52].number = 'B';
	strcpy(m_card[53].shape, "J");
	m_card[53].number = 'C';

	printf("카드 출력\n");
	display_card(m_card);
	printf("\n");
	printf("카드 섞기");
	printf("\n");

	for (i = 0; i < 54; i++)
	{
		rnd = rand() % 54;
		temp = m_card[rnd];
		m_card[rnd] = m_card[i];
		m_card[i] = temp;
	}
	display_card(m_card);
	printf("\n");
	printf("나눈 카드");
	printf("\n");
}
