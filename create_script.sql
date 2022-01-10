create table player
(
    user_id     char(36)    not null
        constraint player_pk
            primary key,
    username    varchar(64) not null,
    password    varchar(90) not null,
    coins       integer default 20,
    active_deck char(36)
);

create unique index player_username_uindex
    on player (username);

create unique index player_user_id_uindex
    on player (user_id);

create table card
(
    card_id      char(36)    not null
        constraint card_pk
            primary key,
    title        varchar(64) not null,
    description  varchar(255),
    damage       integer     not null,
    card_type    integer,
    element_type integer,
    owner        char(36)
        constraint card_player_user_id_fk
            references player
            on delete set null
);

create unique index card_card_id_uindex
    on card (card_id);

create table deck
(
    deck_id char(36) not null
        constraint deck_pk
            primary key,
    owner   char(36) not null
        constraint deck_player_user_id_fk
            references player,
    title   varchar(64) default 'unnamed deck'::character varying
);

alter table player
    add constraint player_deck_deck_id_fk
        foreign key (active_deck) references deck
            on delete set null;

create unique index deck_deck_id_uindex
    on deck (deck_id);

create table cards_in_deck
(
    deck_id char(36) not null
        constraint cards_in_deck_deck_deck_id_fk
            references deck,
    card_id char(36) not null
        constraint cards_in_deck_card_card_id_fk
            references card
            on delete set null,
    constraint cards_in_deck_pk
        unique (deck_id, card_id)
);

create table scoreboard
(
    id           char(36) not null
        constraint scoreboard_pk
            primary key,
    user_id      char(36) not null
        constraint scoreboard_player_user_id_fk
            references player,
    elo          integer default 100,
    games_played integer default 1
);

create unique index scoreboard_id_uindex
    on scoreboard (id);

