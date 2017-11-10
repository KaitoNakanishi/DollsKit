﻿namespace DollsLang
{
    public enum TokenType
    {
        EOF,
        IF, ELIF, ELSE, WHILE,
        ASSIGN,
        ADD_ASSIGN, SUB_ASSIGN, MUL_ASSIGN, DIV_ASSIGN,
        MOD_ASSIGN, POW_ASSIGN,
        LPAREN, RPAREN, LBRACE, RBRACE, LBRACKET, RBRACKET,
        BAR, COMMA,
        PLUS, MINUS, MUL, DIV, MOD, POW,
        LT, LE, GT, GE, EQ, NE,
        AND, OR, NOT,
        NIL, FALSE, TRUE,
        ID, STRING, INT, FLOAT,
    }

    public class Token
    {
        public TokenType Type { get; private set; }
        public string Text { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }

        public Token(TokenType type, string text, int line, int column)
        {
            Type = type;
            Text = text;
            Line = line;
            Column = column;
        }

        public override string ToString()
        {
            return $"[({Line}:{Column}){Type}: {Text}]";
        }
    }
}
