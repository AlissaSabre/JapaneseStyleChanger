﻿// <auto-generated>
// THIS FILE (WNode.cs) IS GENERATED BY T4. DO NOT CHANGE IT. CHANGE THE WNode.tt INSTEAD.
// </auto-generated>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NMeCab;
using NMeCab.Core;

namespace JapaneseStyleChanger
{
    /// <summary>
    /// A MeCab node dedicated for Japanese Style Changer in combination with UniDic 2.2 or later.
    /// </summary>
    public class WNode : MeCabNodeBase<WNode>
    {
		private bool Loaded;

        private void LoadFeatures()
        {
            var features = StrUtils.SplitCsvRow(Feature, 29, 16);

			_Pos1 = features[0];
			_Pos2 = features[1];
			_Pos3 = features[2];
			_Pos4 = features[3];
			_CType = features[4];
			_CForm = features[5];
			_Lemma = features[7];
			_Orth = features[8];
			_OrthBase = features[10];
			_Lid = long.Parse(features[27]);
			_Lemma_id = int.Parse(features[28]);

			Feature = null;
            Loaded = true;
        }


		///<summary>1st classification of part of speech.</summary>
		public string Pos1
		{
			get
			{
				if (!Loaded) LoadFeatures();
				return _Pos1;
			}
		}
		private string _Pos1;

		///<summary>2nd classification of part of speech.</summary>
		public string Pos2
		{
			get
			{
				if (!Loaded) LoadFeatures();
				return _Pos2;
			}
		}
		private string _Pos2;

		///<summary>3rd classification of part of speech.</summary>
		public string Pos3
		{
			get
			{
				if (!Loaded) LoadFeatures();
				return _Pos3;
			}
		}
		private string _Pos3;

		///<summary>4th classificaiton of part of speech.</summary>
		public string Pos4
		{
			get
			{
				if (!Loaded) LoadFeatures();
				return _Pos4;
			}
		}
		private string _Pos4;

		///<summary>Conjugation type.</summary>
		public string CType
		{
			get
			{
				if (!Loaded) LoadFeatures();
				return _CType;
			}
		}
		private string _CType;

		///<summary>Conjugation form.</summary>
		public string CForm
		{
			get
			{
				if (!Loaded) LoadFeatures();
				return _CForm;
			}
		}
		private string _CForm;

		///<summary>Lemma.</summary>
		public string Lemma
		{
			get
			{
				if (!Loaded) LoadFeatures();
				return _Lemma;
			}
		}
		private string _Lemma;

		///<summary>Written text.</summary>
		public string Orth
		{
			get
			{
				if (!Loaded) LoadFeatures();
				return _Orth;
			}
		}
		private string _Orth;

		///<summary>Base form of written text.</summary>
		public string OrthBase
		{
			get
			{
				if (!Loaded) LoadFeatures();
				return _OrthBase;
			}
		}
		private string _OrthBase;

		///<summary>ID in the master table.</summary>
		public long Lid
		{
			get
			{
				if (!Loaded) LoadFeatures();
				return _Lid;
			}
		}
		private long _Lid;

		///<summary>Unique ID of Lemma.</summary>
		public int Lemma_id
		{
			get
			{
				if (!Loaded) LoadFeatures();
				return _Lemma_id;
			}
		}
		private int _Lemma_id;

    }
}
